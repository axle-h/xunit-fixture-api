using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using AutoFixture.Dsl;
using Bogus;

namespace Xunit.Fixture.Api.Extensions
{
    public static class AutoFixtureExtensions
    {
        /// <summary>
        /// Registers that a writable property should be assigned to the result of calling
        /// the specified factory function as part of specimen post-processing.
        /// </summary>
        /// <typeparam name="TModel">The type of the model.</typeparam>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="composer">The composer.</param>
        /// <param name="property">The property.</param>
        /// <param name="factory">The factory.</param>
        /// <returns></returns>
        public static IPostprocessComposer<TModel> With<TModel, TProperty>(this IPostprocessComposer<TModel> composer,
                                                                           Expression<Func<TModel, TProperty>> property,
                                                                           Func<Faker, TProperty> factory)
        {
            var faker = new Faker();
            var info = property.GetProperty();
            return composer.Without(property).Do(x => info.SetValue(x, factory(faker)));
        }
    }
}
