using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.Dsl;
using AutoFixture.Kernel;
using Bogus;

namespace Xunit.Fixture.Api.Extensions
{
    /// <summary>
    /// Extensions for <see cref="IApiTestFixture"/>.
    /// </summary>
    public static class ConvenienceExtensions
    {
        /// <summary>
        /// Performs an arrange action.
        /// </summary>
        /// <param name="fixture">The fixture.</param>
        /// <param name="action">The action.</param>
        /// <returns></returns>
        public static IApiTestFixture Having(this IApiTestFixture fixture, Action action)
        {
            action();
            return fixture;
        }

        /// <summary>
        /// Performs an arrange function, returning the result as an output parameter.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="fixture">The fixture.</param>
        /// <param name="action">The action.</param>
        /// <param name="result">The result.</param>
        /// <returns></returns>
        public static IApiTestFixture Having<TResult>(this IApiTestFixture fixture, Func<TResult> action, out TResult result)
        {
            result = action();
            return fixture;
        }

        /// <summary>
        /// Picks a random model from the specified collection and optionally mutates it.
        /// </summary>
        /// <typeparam name="TModel">The type of the model.</typeparam>
        /// <param name="fixture">The fixture.</param>
        /// <param name="models">The models.</param>
        /// <param name="model">The model.</param>
        /// <param name="mutationFunc">The mutation function.</param>
        /// <returns></returns>
        public static IApiTestFixture HavingRandom<TModel>(this IApiTestFixture fixture,
                                                           ICollection<TModel> models,
                                                           out TModel model,
                                                           Action<TModel> mutationFunc = null)
        {
            model = fixture.Faker.Random.CollectionItem(models);
            mutationFunc?.Invoke(model);
            return fixture;
        }

        /// <summary>
        /// Creates an auto fixture constructed instance of the specified model.
        /// </summary>
        /// <typeparam name="TModel">The type of the model.</typeparam>
        /// <param name="fixture">The fixture.</param>
        /// <param name="model">The model.</param>
        /// <param name="configurator">The configurator.</param>
        /// <returns></returns>
        public static IApiTestFixture HavingModel<TModel>(this IApiTestFixture fixture,
                                                          out TModel model,
                                                          Action<TModel> configurator = null)
        {
            model = fixture.AutoFixture.Create<TModel>();
            configurator?.Invoke(model);
            return fixture;
        }

        /// <summary>
        /// Creates a collection of auto fixture constructed instances of the specified model.
        /// </summary>
        /// <typeparam name="TModel">The type of the model.</typeparam>
        /// <param name="fixture">The fixture.</param>
        /// <param name="models">The models.</param>
        /// <param name="configurator">The configurator.</param>
        /// <returns></returns>
        public static IApiTestFixture HavingModels<TModel>(this IApiTestFixture fixture,
                                                           out ICollection<TModel> models,
                                                           Action<TModel> configurator = null)
        {
            models = fixture.AutoFixture.CreateMany<TModel>().ToList();

            if (configurator != null)
            {
                foreach (var model in models)
                {
                    configurator(model);
                }
            }

            return fixture;
        }

        /// <summary>
        /// Uses the faker on the fixture as a factory for some fake data.
        /// </summary>
        /// <typeparam name="TFake">The type of the fake.</typeparam>
        /// <param name="fixture">The fixture.</param>
        /// <param name="factory">The factory.</param>
        /// <param name="fake">The fake.</param>
        /// <returns></returns>
        public static IApiTestFixture HavingFake<TFake>(this IApiTestFixture fixture,
                                                        Func<Faker, TFake> factory,
                                                        out TFake fake)
        {
            fake = factory(fixture.Faker);
            return fixture;
        }

        /// <summary>
        /// Configures the request to use the specified string as a bearer token in the authorization header.
        /// </summary>
        /// <param name="fixture">The fixture.</param>
        /// <param name="token">The token.</param>
        /// <returns></returns>
        public static IApiTestFixture HavingBearerToken(this IApiTestFixture fixture, string token) =>
            fixture.HavingConfiguredHttpMessage(message => message.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token));

        /// <summary>
        /// Adds the specified composer transformation function as an AutoFixture customization.
        /// </summary>
        /// <typeparam name="TModel">The type of the model.</typeparam>
        /// <param name="fixture">The fixture.</param>
        /// <param name="composer">The composer.</param>
        /// <returns></returns>
        public static IApiTestFixture HavingAutoFixtureCustomization<TModel>(this IApiTestFixture fixture,
                                                                             Func<ICustomizationComposer<TModel>, ISpecimenBuilder> composer)
        {
            fixture.AutoFixture.Customize(composer);
            return fixture;
        }

        /// <summary>
        /// Configures the base URL.
        /// </summary>
        /// <param name="fixture">The fixture.</param>
        /// <param name="baseUrl">The base URL.</param>
        /// <returns></returns>
        public static IApiTestFixture HavingBaseUrl(this IApiTestFixture fixture, string baseUrl) =>
            fixture.HavingClientConfiguration(client => client.BaseAddress = new Uri(baseUrl, UriKind.Absolute));

        /// <summary>
        /// Runs the specified fixture action n times.
        /// </summary>
        /// <param name="fixture">The fixture.</param>
        /// <param name="n">The n.</param>
        /// <param name="func">The function.</param>
        /// <returns></returns>
        public static IApiTestFixture HavingRepeated(this IApiTestFixture fixture, int n, Action<IApiTestFixture> func)
        {
            for (var i = 0; i < n; i++)
            {
                func(fixture);
            }

            return fixture;
        }

        /// <summary>
        /// Configures the fixture perform the specified HTTP action.
        /// </summary>
        /// <param name="fixture">The fixture.</param>
        /// <param name="method">The HTTP method.</param>
        /// <param name="uri">The URI.</param>
        /// <param name="content">The HTTP content.</param>
        /// <returns></returns>
        public static IApiTestFixture When(this IApiTestFixture fixture, HttpMethod method, string uri, HttpContent content = null) =>
            fixture.When(method, () => uri, () => content);

        /// <summary>
        /// Configures the fixture perform the specified HTTP action.
        /// </summary>
        /// <param name="fixture">The fixture.</param>
        /// <param name="method">The HTTP method.</param>
        /// <param name="uriFactory">The URI factory.</param>
        /// <param name="contentFactory">The content factory.</param>
        /// <returns></returns>
        public static IApiTestFixture When(this IApiTestFixture fixture,
                                           HttpMethod method,
                                           Func<string> uriFactory,
                                           Func<HttpContent> contentFactory = null) =>
            fixture.HavingConfiguredHttpMessage(message =>
                                                {
                                                    message.Method = method;
                                                    message.RequestUri = new Uri(uriFactory(), UriKind.Relative);
                                                    message.Content = contentFactory?.Invoke();
                                                });
    }
}
