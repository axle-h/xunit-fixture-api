using System;
using System.Collections.Generic;
using System.Net;
using FluentAssertions;
using FluentAssertions.Equivalency;
using Newtonsoft.Json;

namespace Xunit.Fixture.Api.Extensions
{
    /// <summary>
    /// Extensions for <see cref="IApiTestFixture"/>.
    /// </summary>
    public static class AssertionExtensions
    {
        /// <summary>
        /// Adds assertions that will be run on the HTTP, raw response body.
        /// </summary>
        /// <param name="fixture">The fixture.</param>
        /// <param name="assertions">The assertions.</param>
        /// <returns></returns>
        public static IApiTestFixture ShouldReturnRaw(this IApiTestFixture fixture, params Action<string>[] assertions) =>
            fixture.ShouldReturn(x => x, assertions);

        /// <summary>
        /// Adds an assertion that the response body will be the exact, specified string.
        /// </summary>
        /// <param name="fixture">The fixture.</param>
        /// <param name="expected">The expected.</param>
        /// <returns></returns>
        public static IApiTestFixture ShouldReturnRaw(this IApiTestFixture fixture, string expected) =>
            fixture.ShouldReturn(x => x, x => x.Should().Be(expected));

        /// <summary>
        /// Adds assertions that will be run on the HTTP, JSON response body.
        /// </summary>
        /// <typeparam name="TModel">The type of the result.</typeparam>
        /// <param name="fixture">The fixture.</param>
        /// <param name="assertions">The assertions.</param>
        /// <returns></returns>
        public static IApiTestFixture ShouldReturnJson<TModel>(this IApiTestFixture fixture, params Action<TModel>[] assertions) =>
            fixture.ShouldReturn(JsonConvert.DeserializeObject<TModel>, assertions);

        /// <summary>
        /// Adds an assertion to the specified fixture that the JSON result will be equivalent to the specified model.
        /// </summary>
        /// <typeparam name="TModel">The type of the model.</typeparam>
        /// <param name="fixture">The fixture.</param>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public static IApiTestFixture ShouldReturnEquivalentJson<TModel>(this IApiTestFixture fixture, TModel model) =>
            fixture.ShouldReturnJson<TModel>(r => r.Should().BeEquivalentTo(model));

        /// <summary>
        /// Adds an assertion to the specified fixture that the JSON result will be equivalent to the specified model.
        /// </summary>
        /// <typeparam name="TModel">The type of the model.</typeparam>
        /// <param name="fixture">The fixture.</param>
        /// <param name="model">The model.</param>
        /// <param name="options">The equivalency assertion options.</param>
        /// <returns></returns>
        public static IApiTestFixture ShouldReturnEquivalentJson<TModel>(this IApiTestFixture fixture,
                                                                         TModel model,
                                                                         Func<EquivalencyAssertionOptions<TModel>, EquivalencyAssertionOptions<TModel>> options) =>
            fixture.ShouldReturnJson<TModel>(r => r.Should().BeEquivalentTo(model, options));

        /// <summary>
        /// Adds an assertion to the specified fixture that the JSON result will be equivalent to the specified model.
        /// </summary>
        /// <typeparam name="TResponseModel">The type of the response model.</typeparam>
        /// <typeparam name="TModel">The type of the model.</typeparam>
        /// <param name="fixture">The fixture.</param>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public static IApiTestFixture ShouldReturnEquivalentJson<TResponseModel, TModel>(this IApiTestFixture fixture, TModel model) =>
            fixture.ShouldReturnJson<TResponseModel>(x => x.Should().BeEquivalentTo(model));

        /// <summary>
        /// Adds an assertion to the specified fixture that the JSON result will be equivalent to the specified model.
        /// </summary>
        /// <typeparam name="TResponseModel">The type of the response model.</typeparam>
        /// <typeparam name="TModel">The type of the model.</typeparam>
        /// <param name="fixture">The fixture.</param>
        /// <param name="model">The model.</param>
        /// <param name="options">The equivalency assertion options.</param>
        /// <returns></returns>
        public static IApiTestFixture ShouldReturnEquivalentJson<TResponseModel, TModel>(this IApiTestFixture fixture,
                                                                                         TModel model,
                                                                                         Func<EquivalencyAssertionOptions<TModel>, EquivalencyAssertionOptions<TModel>> options) =>
            fixture.ShouldReturnJson<TResponseModel>(x => x.Should().BeEquivalentTo(model, options));

        /// <summary>
        /// Adds an assertion to the specified fixture that the JSON result returned will be an empty collection.
        /// </summary>
        /// <param name="fixture">The fixture.</param>
        /// <returns></returns>
        public static IApiTestFixture ShouldReturnEmptyJsonCollection(this IApiTestFixture fixture) =>
            fixture.ShouldReturnJson<ICollection<object>>(x => x.Should().BeEmpty());

        /// <summary>
        /// Adds an assertion to the specified fixture that the JSON result returned will be a collection of the specified length.
        /// </summary>
        /// <param name="fixture">The fixture.</param>
        /// <param name="count">The count.</param>
        /// <returns></returns>
        public static IApiTestFixture ShouldReturnJsonCollectionOfLength(this IApiTestFixture fixture, int count) =>
            fixture.ShouldReturnJson<ICollection<object>>(x => x.Should().HaveCount(count));

        /// <summary>
        /// Adds an assert step that the HTTP response had a success status code i.e. 2xx.
        /// </summary>
        /// <param name="fixture">The fixture.</param>
        /// <returns></returns>
        public static IApiTestFixture ShouldReturnSuccessfulStatus(this IApiTestFixture fixture) =>
            fixture.ShouldReturnResponse(r => r.EnsureSuccessStatusCode());

        /// <summary>
        /// Adds an assert step that the HTTP response was a permanent redirect (301) to the specified url.
        /// </summary>
        /// <param name="fixture">The fixture.</param>
        /// <param name="redirectUrl">The redirect URL.</param>
        /// <returns></returns>
        public static IApiTestFixture ShouldReturnPermanentRedirect(this IApiTestFixture fixture, string redirectUrl)
            => fixture.ShouldReturnRedirect(HttpStatusCode.Moved, redirectUrl);

        /// <summary>
        /// Adds an assert step that the HTTP response was a redirect (302) to the specified url.
        /// </summary>
        /// <param name="fixture">The fixture.</param>
        /// <param name="redirectUrl">The redirect URL.</param>
        /// <returns></returns>
        public static IApiTestFixture ShouldReturnRedirect(this IApiTestFixture fixture, string redirectUrl)
            => fixture.ShouldReturnRedirect(HttpStatusCode.Redirect, redirectUrl);

        /// <summary>
        /// Adds an assert step that the HTTP response was a see other (303 - redirect to a GET) to the specified url.
        /// </summary>
        /// <param name="fixture">The fixture.</param>
        /// <param name="redirectUrl">The redirect URL.</param>
        /// <returns></returns>
        public static IApiTestFixture ShouldReturnSeeOther(this IApiTestFixture fixture, string redirectUrl)
            => fixture.ShouldReturnRedirect(HttpStatusCode.SeeOther, redirectUrl);

        /// <summary>
        /// Adds an assert step that the HTTP response was a temporary redirect (307) to the specified url.
        /// </summary>
        /// <param name="fixture">The fixture.</param>
        /// <param name="redirectUrl">The redirect URL.</param>
        /// <returns></returns>
        public static IApiTestFixture ShouldReturnTemporaryRedirect(this IApiTestFixture fixture, string redirectUrl)
            => fixture.ShouldReturnRedirect(HttpStatusCode.TemporaryRedirect, redirectUrl);

        /// <summary>
        /// Adds an assert step that the HTTP response had a bad request (400) status code.
        /// </summary>
        /// <param name="fixture">The fixture.</param>
        /// <returns></returns>
        public static IApiTestFixture ShouldReturnBadRequest(this IApiTestFixture fixture)
            => fixture.ShouldReturnStatus(HttpStatusCode.BadRequest);

        /// <summary>
        /// Adds an assert step that the HTTP response had an unauthorized (401) status code.
        /// </summary>
        /// <param name="fixture">The fixture.</param>
        /// <returns></returns>
        public static IApiTestFixture ShouldReturnUnauthorized(this IApiTestFixture fixture)
            => fixture.ShouldReturnStatus(HttpStatusCode.Unauthorized);

        /// <summary>
        /// Adds an assert step that the HTTP response had a forbidden (403) status code.
        /// </summary>
        /// <param name="fixture">The fixture.</param>
        /// <returns></returns>
        public static IApiTestFixture ShouldReturnForbidden(this IApiTestFixture fixture)
            => fixture.ShouldReturnStatus(HttpStatusCode.Forbidden);

        /// <summary>
        /// Adds an assert step that the HTTP response had a not found (404) status code.
        /// </summary>
        /// <param name="fixture">The fixture.</param>
        /// <returns></returns>
        public static IApiTestFixture ShouldReturnNotFound(this IApiTestFixture fixture)
            => fixture.ShouldReturnStatus(HttpStatusCode.NotFound);

        /// <summary>
        /// Adds an assert step that the HTTP response had a internal server error (500) status code.
        /// </summary>
        /// <param name="fixture">The fixture.</param>
        /// <returns></returns>
        public static IApiTestFixture ShouldReturnInternalServerError(this IApiTestFixture fixture)
            => fixture.ShouldReturnStatus(HttpStatusCode.InternalServerError);

        /// <summary>
        /// Adds an assert step that the HTTP response has the specified status code.
        /// </summary>
        /// <param name="fixture">The fixture.</param>
        /// <param name="statusCode">The status code.</param>
        /// <returns></returns>
        public static IApiTestFixture ShouldReturnStatus(this IApiTestFixture fixture, HttpStatusCode statusCode) =>
            fixture.ShouldReturnResponse(r => r.StatusCode.Should().Be(statusCode));

        private static IApiTestFixture ShouldReturnRedirect(this IApiTestFixture fixture, HttpStatusCode statusCode, string redirectUrl) =>
            fixture.ShouldReturnResponse(r => r.StatusCode.Should().Be(statusCode),
                                   r => r.Headers.Location.Should().Be(redirectUrl));
    }
}
