using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Xunit.Fixture.Api.Infrastructure;

namespace Xunit.Fixture.Api.Extensions
{
    public static class PostRequestExtensions
    {
        public static IApiTestFixture ShouldSatisfyRequest(this IApiTestFixture fixture,
                                                               HttpMethod method,
                                                               string path,
                                                               object body = null,
                                                               Func<HttpResponseMessage, Task> assertion = null)
        {
            var content = body == null ? null : new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");
            return fixture.ShouldSatisfyRequest(message =>
                                                {
                                                    message.Method = method;
                                                    message.RequestUri = new Uri(path, UriKind.Relative);
                                                    message.Content = content;
                                                }, assertion);
        }

        public static IApiTestFixture ShouldSatisfyRawGetRequest(this IApiTestFixture fixture, string path, params Action<string>[] assertions) =>
            fixture.ShouldSatisfyRequest(HttpMethod.Get, path, null, m => RunRawResponseAssertion(m, assertions));

        public static IApiTestFixture ShouldSatisfyJsonGetRequest<TResponse>(this IApiTestFixture fixture, string path, params Action<TResponse>[] assertions) =>
            fixture.ShouldSatisfyRequest(HttpMethod.Get, path, null, m => RunJsonResponseAssertion(m, assertions));

        public static IApiTestFixture ShouldSatisfyJsonGetRequest<TResponse>(this IApiTestFixture fixture,
                                                                             string path,
                                                                             object query,
                                                                             params Action<TResponse>[] assertions) =>
            fixture.ShouldSatisfyJsonGetRequest(UrlHelper.GetWithQuery(path, query), assertions);

        public static IApiTestFixture ShouldSatisfyRawGetRequest(this IApiTestFixture fixture,
                                                                 string path,
                                                                 object query,
                                                                 params Action<string>[] assertions) =>
            fixture.ShouldSatisfyRawGetRequest(UrlHelper.GetWithQuery(path, query), assertions);

        public static IApiTestFixture ShouldSatisfyJsonPostRequest<TResponse>(this IApiTestFixture fixture,
                                                                              string path,
                                                                              object body,
                                                                              params Action<TResponse>[] assertions) =>
            fixture.ShouldSatisfyRequest(HttpMethod.Post, path, body, m => RunJsonResponseAssertion(m, assertions));

        public static IApiTestFixture ShouldSatisfyRawPostRequest(this IApiTestFixture fixture,
                                                                  string path,
                                                                  object body,
                                                                  params Action<string>[] assertions) =>
            fixture.ShouldSatisfyRequest(HttpMethod.Post, path, body, m => RunRawResponseAssertion(m, assertions));

        private static async Task RunJsonResponseAssertion<TResponse>(HttpResponseMessage message, IEnumerable<Action<TResponse>> assertions)
        {
            message.EnsureSuccessStatusCode();
            var responseBody = await message.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<TResponse>(responseBody);

            using (var aggregator = new ExceptionAggregator())
            {
                foreach (var assertion in assertions)
                {
                    aggregator.Try(() => assertion(result));
                }
            }
        }

        private static async Task RunRawResponseAssertion(HttpResponseMessage message, IEnumerable<Action<string>> assertions)
        {
            message.EnsureSuccessStatusCode();
            var responseBody = await message.Content.ReadAsStringAsync();
            using (var aggregator = new ExceptionAggregator())
            {
                foreach (var assertion in assertions)
                {
                    aggregator.Try(() => assertion(responseBody));
                }
            }
        }
    }
}