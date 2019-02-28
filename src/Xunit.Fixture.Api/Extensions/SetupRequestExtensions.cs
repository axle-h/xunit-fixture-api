using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Xunit.Fixture.Api.Extensions
{
    public static class SetupRequestExtensions
    {
        /// <summary>
        /// Configures and adds an HTTP request message to be sent before the test request is sent.
        /// </summary>
        /// <param name="fixture">The fixture.</param>
        /// <param name="method">The method.</param>
        /// <param name="uri">The URI.</param>
        /// <param name="content">The content.</param>
        /// <param name="assertion">The assertion.</param>
        /// <returns></returns>
        public static IApiTestFixture HavingPreviouslySentRequest(this IApiTestFixture fixture,
                                                                  HttpMethod method,
                                                                  string uri,
                                                                  HttpContent content,
                                                                  Func<HttpResponseMessage, Task> assertion = null) =>
            fixture.HavingSetupRequest(message =>
                                       {
                                           message.Method = method;
                                           message.RequestUri = new Uri(uri, UriKind.Relative);
                                           message.Content = content;
                                       }, assertion);

        /// <summary>
        /// Adds a POST request to the specified fixture to the specified URL and with the specified body, to be run during the setup step.
        /// The optionally specified assertion should be used to assert that the response was successful.
        /// </summary>
        /// <typeparam name="TBody">The type of the body.</typeparam>
        /// <param name="fixture">The fixture.</param>
        /// <param name="path">The path.</param>
        /// <param name="body">The body.</param>
        /// <param name="assertion">The assertion.</param>
        /// <returns></returns>
        public static IApiTestFixture HavingPreviouslyCreated<TBody>(this IApiTestFixture fixture,
                                                                     string path,
                                                                     TBody body,
                                                                     Func<HttpResponseMessage, Task> assertion = null) =>
            fixture.HavingPreviouslySentJsonRequest(HttpMethod.Post, path, body, assertion);

        /// <summary>
        /// Adds a POST request to the specified fixture to the specified URL and with the specified body, to be run during the setup step.
        /// The optionally specified assertion should be used to assert that the response was successful.
        /// </summary>
        /// <typeparam name="TBody">The type of the body.</typeparam>
        /// <param name="fixture">The fixture.</param>
        /// <param name="path">The path.</param>
        /// <param name="body">The body.</param>
        /// <param name="assertion">The assertion.</param>
        /// <param name="configurator">The configurator.</param>
        /// <returns></returns>
        public static IApiTestFixture HavingPreviouslyCreated<TBody>(this IApiTestFixture fixture,
                                                                     string path,
                                                                     out TBody body,
                                                                     Func<HttpResponseMessage, Task> assertion = null,
                                                                     Action<TBody> configurator = null) =>
            fixture.HavingModel(out body, configurator)
                   .HavingPreviouslyCreated(path, body, assertion);

        /// <summary>
        /// Adds a GET request to the specified fixture to the specified URL with the specified query string to be run during the setup step.
        /// The result will be returned via the specified response getter function.
        /// This should not be called before the fixture has been run.
        /// </summary>
        /// <typeparam name="TQuery">The type of the query.</typeparam>
        /// <typeparam name="TResponse">The type of the response.</typeparam>
        /// <param name="fixture">The fixture.</param>
        /// <param name="path">The path.</param>
        /// <param name="query">The query.</param>
        /// <param name="responseGetter">The response getter.</param>
        /// <returns></returns>
        public static IApiTestFixture HavingPreviouslyRetrieved<TResponse>(this IApiTestFixture fixture,
                                                                           string path,
                                                                           object query,
                                                                           out Func<TResponse> responseGetter)
            where TResponse : class =>
            fixture.HavingPreviouslyRetrieved(UrlHelper.GetWithQuery(path, query), out responseGetter);

        /// <summary>
        /// Adds a GET request to the specified fixture to the specified URL to be run during the setup step.
        /// The result will be returned via the specified response getter function.
        /// This should not be called before the fixture has been run.
        /// </summary>
        /// <typeparam name="TResponse">The type of the response.</typeparam>
        /// <param name="fixture">The fixture.</param>
        /// <param name="path">The path.</param>
        /// <param name="responseGetter">The response getter.</param>
        /// <returns></returns>
        public static IApiTestFixture HavingPreviouslyRetrieved<TResponse>(this IApiTestFixture fixture,
                                                                           string path,
                                                                           out Func<TResponse> responseGetter)
            where TResponse : class
        {
            TResponse response = null;
            responseGetter = () =>
                             {
                                 if (response == null)
                                 {
                                     throw new InvalidOperationException("Cannot call setup getter until after the setup step");
                                 }

                                 return response;
                             };

            return fixture.HavingPreviouslySentRequest(HttpMethod.Get,
                                                       path,
                                                       null,
                                                       async r =>
                                                       {
                                                           var content = await r.Content.ReadAsStringAsync();
                                                           response = JsonConvert.DeserializeObject<TResponse>(content);
                                                       });
        }

        /// <summary>
        /// Configures the specified fixture's act step to be an HTTP request with the specified method, url and body.
        /// </summary>
        /// <param name="fixture">The fixture.</param>
        /// <param name="method">The method.</param>
        /// <param name="path">The path.</param>
        /// <param name="body">The body.</param>
        /// <param name="assertion">The assertion.</param>
        /// <returns></returns>
        public static IApiTestFixture HavingPreviouslySentJsonRequest(this IApiTestFixture fixture,
                                                                      HttpMethod method,
                                                                      string path,
                                                                      object body = null,
                                                                      Func<HttpResponseMessage, Task> assertion = null)
        {
            var content = body == null ? null : new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");
            return fixture.HavingPreviouslySentRequest(method, path, content, assertion);
        }
    }
}
