using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using AutoFixture;
using Bogus;

namespace Xunit.Fixture.Api
{
    public interface IApiTestFixture
    {
        /// <summary>
        /// Gets the auto fixture.
        /// </summary>
        /// <value>
        /// The auto fixture.
        /// </value>
        IFixture AutoFixture { get; }

        /// <summary>
        /// Gets the faker.
        /// </summary>
        Faker Faker { get; }

        /// <summary>
        /// Gets the properties.
        /// </summary>
        IDictionary<string, object> Properties { get; }

        /// <summary>
        /// Adds the specified configurator for the test client.
        /// </summary>
        /// <param name="configurator">The configurator.</param>
        /// <returns></returns>
        IApiTestFixture HavingClientConfiguration(Action<HttpClient> configurator);

        /// <summary>
        /// Configures and adds an HTTP request message to be sent before the test request is sent.
        /// </summary>
        /// <param name="configurator">The configurator.</param>
        /// <param name="assertion">The assertion.</param>
        /// <returns></returns>
        IApiTestFixture HavingSetupRequest(Action<HttpRequestMessage> configurator,
                                                           Func<HttpResponseMessage, Task> assertion = null);

        /// <summary>
        /// Configures the HTTP request message.
        /// </summary>
        /// <param name="configurator">The configurator.</param>
        /// <returns></returns>
        IApiTestFixture HavingConfiguredHttpMessage(Action<HttpRequestMessage> configurator);

        /// <summary>
        /// Adds assertions that will be run on the HTTP response.
        /// </summary>
        /// <param name="assertions">The assertions.</param>
        IApiTestFixture ShouldReturnResponse(params Action<HttpResponseMessage>[] assertions);

        /// <summary>
        /// Configures and adds an HTTP request message to be sent after the test request is sent.
        /// This should be used to validate actions that occurred during 
        /// </summary>
        /// <param name="configurator">The configurator.</param>
        /// <param name="assertion">The assertion.</param>
        /// <returns></returns>
        IApiTestFixture ShouldSatisfyRequest(Action<HttpRequestMessage> configurator,
                                                             Func<HttpResponseMessage, Task> assertion = null);

        /// <summary>
        /// Adds an assertion that the response body should be successfully deserialized using the specified deserializer
        /// and then satisfy the specified assertion actions.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="deserializer">The deserializer.</param>
        /// <param name="assertions">The assertions.</param>
        /// <returns></returns>
        IApiTestFixture ShouldReturn<TResult>(Func<string, TResult> deserializer, params Action<TResult>[] assertions);

        /// <summary>
        /// Runs the fixture.
        /// </summary>
        /// <returns></returns>
        Task RunAsync();
    }
}