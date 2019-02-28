using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;

namespace Xunit.Fixture.Api.Extensions
{
    /// <summary>
    /// Extensions for configuring an <see cref="IApiTestFixture"/>.
    /// </summary>
    public static class RestExtensions
    {
        private static readonly HttpMethod Patch = new HttpMethod("PATCH");

        /// <summary>
        /// Configures the specified fixture's act step to be a GET request at the specified url.
        /// </summary>
        /// <param name="fixture">The fixture.</param>
        /// <param name="url">The URL.</param>
        /// <returns></returns>
        public static IApiTestFixture WhenGetting(this IApiTestFixture fixture, string url) =>
            fixture.When(HttpMethod.Get, url);

        /// <summary>
        /// Configures the specified fixture's act step to be a GET request at the specified url.
        /// </summary>
        /// <typeparam name="TQuery">The type of the query.</typeparam>
        /// <param name="fixture">The fixture.</param>
        /// <param name="url">The URL.</param>
        /// <param name="queryFactory">The query factory.</param>
        /// <returns></returns>
        public static IApiTestFixture WhenGetting<TQuery>(this IApiTestFixture fixture, string url, Func<TQuery> queryFactory) =>
            fixture.When(HttpMethod.Get, () => UrlHelper.GetWithQuery(url, queryFactory()));

        /// <summary>
        /// Configures the specified fixture's act step to be a GET request at the specified url.
        /// </summary>
        /// <typeparam name="TQuery">The type of the query.</typeparam>
        /// <param name="fixture">The fixture.</param>
        /// <param name="url">The URL.</param>
        /// <param name="query">The query.</param>
        /// <returns></returns>
        public static IApiTestFixture WhenGetting<TQuery>(this IApiTestFixture fixture, string url, TQuery query) =>
            fixture.When(HttpMethod.Get, UrlHelper.GetWithQuery(url, query));

        /// <summary>
        /// Configures the specified fixture's act step to be a GET request for the specified entity and id.
        /// </summary>
        /// <typeparam name="TId">The type of the identifier.</typeparam>
        /// <param name="fixture">The fixture.</param>
        /// <param name="entity">The entity.</param>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public static IApiTestFixture WhenGettingById<TId>(this IApiTestFixture fixture, string entity, TId id) =>
            fixture.When(HttpMethod.Get, $"{entity}/{Uri.EscapeDataString(id.ToString())}");

        /// <summary>
        /// Configures the specified fixture's act step to be a PUT request for the specified entity and id with the specified JSON body.
        /// </summary>
        /// <typeparam name="TId">The type of the identifier.</typeparam>
        /// <typeparam name="TBody">The type of the body.</typeparam>
        /// <param name="fixture">The fixture.</param>
        /// <param name="entity">The entity.</param>
        /// <param name="id">The identifier.</param>
        /// <param name="body">The body.</param>
        /// <returns></returns>
        public static IApiTestFixture WhenUpdating<TId, TBody>(this IApiTestFixture fixture, string entity, TId id, TBody body) =>
            fixture.WhenCallingRestMethod(HttpMethod.Put, $"{entity}/{Uri.EscapeDataString(id.ToString())}", body);

        /// <summary>
        /// Configures the specified fixture's act step to be a PUT request for the specified entity with the specified JSON body.
        /// </summary>
        /// <typeparam name="TBody">The type of the body.</typeparam>
        /// <param name="fixture">The fixture.</param>
        /// <param name="entity">The entity.</param>
        /// <param name="body">The body.</param>
        /// <returns></returns>
        public static IApiTestFixture WhenUpdating<TBody>(this IApiTestFixture fixture, string entity, TBody body) =>
            fixture.WhenCallingRestMethod(HttpMethod.Put, entity, body);

        /// <summary>
        /// Configures the specified fixture's act step to be a PUT request for the specified entity and id with the specified JSON body.
        /// </summary>
        /// <typeparam name="TId">The type of the identifier.</typeparam>
        /// <typeparam name="TModel">The type of the model.</typeparam>
        /// <param name="fixture">The fixture.</param>
        /// <param name="entity">The entity.</param>
        /// <param name="id">The identifier.</param>
        /// <param name="model">The model.</param>
        /// <param name="configurator">The configurator.</param>
        /// <returns></returns>
        public static IApiTestFixture WhenUpdating<TId, TModel>(this IApiTestFixture fixture,
                                                                string entity,
                                                                TId id,
                                                                out TModel model,
                                                                Action<TModel> configurator = null) =>
            fixture.WhenCallingRestMethod(HttpMethod.Put, $"{entity}/{Uri.EscapeDataString(id.ToString())}", out model, configurator);

        /// <summary>
        /// Configures the specified fixture's act step to be a PUT request for the specified entity with the specified JSON body.
        /// </summary>
        /// <typeparam name="TModel">The type of the model.</typeparam>
        /// <param name="fixture">The fixture.</param>
        /// <param name="entity">The entity.</param>
        /// <param name="model">The model.</param>
        /// <param name="configurator">The configurator.</param>
        /// <returns></returns>
        public static IApiTestFixture WhenUpdating<TModel>(this IApiTestFixture fixture,
                                                           string entity,
                                                           out TModel model,
                                                           Action<TModel> configurator = null) =>
            fixture.WhenCallingRestMethod(HttpMethod.Put, entity, out model, configurator);

        /// <summary>
        /// Configures the specified fixture's act step to be a POST request for the specified entity with the specified JSON body.
        /// </summary>
        /// <typeparam name="TBody">The type of the body.</typeparam>
        /// <param name="fixture">The fixture.</param>
        /// <param name="entity">The entity.</param>
        /// <param name="body">The body.</param>
        /// <returns></returns>
        public static IApiTestFixture WhenCreating<TBody>(this IApiTestFixture fixture, string entity, TBody body) =>
            fixture.WhenCallingRestMethod(HttpMethod.Post, entity, body);


        /// <summary>
        /// Configures the specified fixture's act step to be a POST request for the specified entity with the specified JSON body.
        /// </summary>
        /// <typeparam name="TModel">The type of the model.</typeparam>
        /// <param name="fixture">The fixture.</param>
        /// <param name="entity">The entity.</param>
        /// <param name="model">The model.</param>
        /// <param name="configurator">The configurator.</param>
        /// <returns></returns>
        public static IApiTestFixture WhenCreating<TModel>(this IApiTestFixture fixture, string entity, out TModel model, Action<TModel> configurator = null) =>
            fixture.WhenCallingRestMethod(HttpMethod.Post, entity, out model, configurator);

        /// <summary>
        /// Configures the specified fixture's act step to be a PATCH request for the specified entity and id with the specified JSON body.
        /// </summary>
        /// <typeparam name="TId">The type of the identifier.</typeparam>
        /// <typeparam name="TBody">The type of the body.</typeparam>
        /// <param name="fixture">The fixture.</param>
        /// <param name="entity">The entity.</param>
        /// <param name="id">The identifier.</param>
        /// <param name="body">The body.</param>
        /// <returns></returns>
        public static IApiTestFixture WhenPatching<TId, TBody>(this IApiTestFixture fixture, string entity, TId id, TBody body) =>
            fixture.WhenCallingRestMethod(Patch, $"{entity}/{Uri.EscapeDataString(id.ToString())}", body);

        /// <summary>
        /// Configures the specified fixture's act step to be a PATCH request for the specified entity and id with the specified JSON body.
        /// </summary>
        /// <typeparam name="TId">The type of the identifier.</typeparam>
        /// <typeparam name="TModel">The type of the model.</typeparam>
        /// <param name="fixture">The fixture.</param>
        /// <param name="entity">The entity.</param>
        /// <param name="id">The identifier.</param>
        /// <param name="model">The model.</param>
        /// <param name="configurator">The configurator.</param>
        /// <returns></returns>
        public static IApiTestFixture WhenPatching<TId, TModel>(this IApiTestFixture fixture,
                                                                string entity,
                                                                TId id,
                                                                out TModel model,
                                                                Action<TModel> configurator = null) =>
            fixture.WhenCallingRestMethod(Patch, $"{entity}/{Uri.EscapeDataString(id.ToString())}", out model, configurator);

        /// <summary>
        /// Configures the specified fixture's act step to be a DELETE request for the specified entity and id.
        /// </summary>
        /// <typeparam name="TId">The type of the identifier.</typeparam>
        /// <param name="fixture">The fixture.</param>
        /// <param name="entity">The entity.</param>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public static IApiTestFixture WhenDeleting<TId>(this IApiTestFixture fixture, string entity, TId id) =>
            fixture.WhenCallingRestMethod(HttpMethod.Delete, $"{entity}/{Uri.EscapeDataString(id.ToString())}");

        /// <summary>
        /// Configures the specified fixture's act step to be an HTTP request with the specified method, url and body.
        /// </summary>
        /// <param name="fixture">The fixture.</param>
        /// <param name="method">The method.</param>
        /// <param name="url">The URL.</param>
        /// <param name="body">The body.</param>
        /// <returns></returns>
        public static IApiTestFixture WhenCallingRestMethod(this IApiTestFixture fixture, HttpMethod method, string url, object body = null) =>
            fixture.When(method, url, GetJsonContent(body));

        /// <summary>
        /// Configures the specified fixture's act step to be an HTTP request with the specified method, url and body.
        /// </summary>
        /// <typeparam name="TModel">The type of the model.</typeparam>
        /// <param name="fixture">The fixture.</param>
        /// <param name="method">The method.</param>
        /// <param name="url">The URL.</param>
        /// <param name="model">The model.</param>
        /// <param name="configurator">The configurator.</param>
        /// <returns></returns>
        public static IApiTestFixture WhenCallingRestMethod<TModel>(this IApiTestFixture fixture,
                                                                    HttpMethod method,
                                                                    string url,
                                                                    out TModel model,
                                                                    Action<TModel> configurator) =>
            fixture.HavingModel(out model, configurator)
                   .WhenCallingRestMethod(method, url, model);

        private static HttpContent GetJsonContent(object body) =>
            body == null ? null : new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");
    }
}
