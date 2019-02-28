using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AutoFixture;
using Bogus;
using FluentAssertions;
using Xunit.Fixture.Api.Extensions;
using Xunit.Abstractions;
using Xunit.Fixture.Api.Infrastructure;

namespace Xunit.Fixture.Api
{
    public class ApiTestFixture : IApiTestFixture
    {
        private readonly Stopwatch _stopwatch = Stopwatch.StartNew();
        private readonly ITestOutputHelper _output;
        private readonly IList<Action<HttpClient>> _clientConfigurationDelegates = new List<Action<HttpClient>>();
        private readonly IList<(HttpRequestMessage message, Func<HttpResponseMessage, Task> assertion)> _setupActions =
            new List<(HttpRequestMessage message, Func<HttpResponseMessage, Task> assertion)>();
        private readonly IList<(Action<HttpRequestMessage> messageConfigurator, Func<HttpResponseMessage, Task> assertion)> _postRequestActions =
            new List<(Action<HttpRequestMessage> messageConfigurator, Func<HttpResponseMessage, Task> assertion)>();
        private readonly IList<Action<HttpResponseMessage>> _responseAssertions = new List<Action<HttpResponseMessage>>();
        private readonly IList<(Func<string, object> deserializer, IList<Action<object>> assertions)> _resultAssertions =
            new List<(Func<string, object> deserializer, IList<Action<object>> assertions)>();
        private readonly IList<Action<HttpRequestMessage>> _messageConfigurationDelegates = new List<Action<HttpRequestMessage>>();

        private static readonly Func<HttpResponseMessage, Task> DefaultAssertion = r =>
                                                                                   {
                                                                                       r.EnsureSuccessStatusCode();
                                                                                       return Task.CompletedTask;
                                                                                   };

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiTestFixture"/> class.
        /// </summary>
        /// <param name="output">The output.</param>
        public ApiTestFixture(ITestOutputHelper output)
        {
            _output = output;
        }

        /// <summary>
        /// Gets the auto fixture.
        /// </summary>
        /// <value>
        /// The auto fixture.
        /// </value>
        public IFixture AutoFixture { get; } = new AutoFixture.Fixture();

        /// <summary>
        /// Gets the faker.
        /// </summary>
        public Faker Faker { get; } = new Faker();

        /// <summary>
        /// Gets the properties.
        /// </summary>
        public IDictionary<string, object> Properties { get; } = new Dictionary<string, object>();

        /// <summary>
        /// Adds the specified configurator for the test client.
        /// </summary>
        /// <param name="configurator">The configurator.</param>
        /// <returns></returns>
        public IApiTestFixture HavingClientConfiguration(Action<HttpClient> configurator) =>
            this.Having(() => _clientConfigurationDelegates.Add(configurator));

        /// <summary>
        /// Configures and adds an HTTP request message to be sent before the test request is sent.
        /// </summary>
        /// <param name="configurator">The configurator.</param>
        /// <param name="assertion">The assertion.</param>
        /// <returns></returns>
        public IApiTestFixture HavingSetupRequest(Action<HttpRequestMessage> configurator,
                                                  Func<HttpResponseMessage, Task> assertion = null) =>
            this.Having(() =>
                   {
                       var message = new HttpRequestMessage();
                       configurator(message);
                       _setupActions.Add((message, assertion ?? DefaultAssertion));
                   });

        /// <summary>
        /// Configures the HTTP request message.
        /// </summary>
        /// <param name="configurator">The configurator.</param>
        /// <returns></returns>
        public IApiTestFixture HavingConfiguredHttpMessage(Action<HttpRequestMessage> configurator) =>
            this.Having(() => _messageConfigurationDelegates.Add(configurator));

        /// <summary>
        /// Adds assertions that will be run on the HTTP response.
        /// </summary>
        /// <param name="assertions">The assertions.</param>
        public IApiTestFixture ShouldReturnResponse(params Action<HttpResponseMessage>[] assertions)
        {
            foreach (var assertion in assertions)
            {
                _responseAssertions.Add(assertion);
            }

            return this;
        }

        /// <summary>
        /// Configures and adds an HTTP request message to be sent after the test request is sent.
        /// This should be used to validate actions that occurred during 
        /// </summary>
        /// <param name="configurator">The configurator.</param>
        /// <param name="assertion">The assertion.</param>
        /// <returns></returns>
        public IApiTestFixture ShouldSatisfyRequest(Action<HttpRequestMessage> configurator,
                                                       Func<HttpResponseMessage, Task> assertion = null) =>
            this.Having(() => _postRequestActions.Add((configurator, assertion ?? DefaultAssertion)));

        /// <summary>
        /// Adds an assertion that the response body should be successfully deserialized using the specified deserializer
        /// and then satisfy the specified assertion actions.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="deserializer">The deserializer.</param>
        /// <param name="assertions">The assertions.</param>
        /// <returns></returns>
        public IApiTestFixture ShouldReturn<TResult>(Func<string, TResult> deserializer, params Action<TResult>[] assertions)
        {
            var objectAssertions = assertions.Select<Action<TResult>, Action<object>>(a => o => a(o.Should().BeAssignableTo<TResult>().Which))
                                             .ToList();
            _resultAssertions.Add((s => deserializer(s), objectAssertions));
            return this;
        }

        /// <summary>
        /// Runs the fixture.
        /// </summary>
        /// <returns></returns>
        public async Task RunAsync()
        {
            if (!_messageConfigurationDelegates.Any())
            {
                throw new InvalidOperationException($"Must call {nameof(HavingConfiguredHttpMessage)} to configure an HTTP request");
            }

            using (var client = new HttpClient())
            {
                foreach (var clientConfigurator in _clientConfigurationDelegates)
                {
                    clientConfigurator(client);
                }

                foreach (var (message, assertion) in _setupActions)
                {
                    await LogAsync(message);

                    var response = await client.SendAsync(message);

                    await LogAsync(response);

                    await assertion(response);
                }

                using (var aggregator = new ExceptionAggregator())
                {
                    var message = new HttpRequestMessage();
                    foreach (var action in _messageConfigurationDelegates)
                    {
                        action(message);
                    }

                    await LogAsync(message);

                    var response = await client.SendAsync(message);
                    var responseBody = await response.Content.ReadAsStringAsync();
                    await LogAsync(response);

                    // Response assertions.
                    foreach (var assertion in _responseAssertions)
                    {
                        aggregator.Try(() => assertion(response));
                    }

                    foreach (var (deserializer, assertions) in _resultAssertions)
                    {
                        try
                        {
                            var result = deserializer(responseBody);
                            foreach (var assertion in assertions)
                            {
                                aggregator.Try(() => assertion(result));
                            }
                        }
                        catch (Exception e)
                        {
                            aggregator.Add(e);
                        }
                    }

                    foreach (var (messageConfigurator, assertion) in _postRequestActions)
                    {
                        var postRequestMessage = new HttpRequestMessage();
                        messageConfigurator(postRequestMessage);

                        await LogAsync(postRequestMessage);

                        var postRequestResponse = await client.SendAsync(postRequestMessage);

                        await LogAsync(postRequestResponse);

                        await assertion(postRequestResponse);
                    }
                }
            }    
        }

        private async Task LogAsync(HttpRequestMessage message)
        {
            var requestBody = await (message.Content?.ReadAsStringAsync() ?? Task.FromResult<string>(null));
            Log($"Sending {message}\n{requestBody ?? "<no body>"}");
        }

        private async Task LogAsync(HttpResponseMessage message)
        {
            var requestBody = await (message.Content?.ReadAsStringAsync() ?? Task.FromResult<string>(null));
            Log($"Received {message}\n{requestBody ?? "<no body>"}");
        }

        private void Log(string message) => _output.WriteLine($"{_stopwatch.Elapsed} {message}");
    }
}
