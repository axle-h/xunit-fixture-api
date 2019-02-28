using System;
using Xunit.Abstractions;
using Xunit.Fixture.Api.Extensions;

namespace Xunit.Fixture.Api.Example.Tests
{
    public abstract class TestBase
    {
        private readonly ITestOutputHelper _output;

        protected TestBase(ITestOutputHelper output)
        {
            _output = output;
        }

        protected IApiTestFixture GivenDateApiTestFixture() =>
            new ApiTestFixture(_output)
               .HavingBaseUrl(Environment.GetEnvironmentVariable("API_URL") ?? "http://localhost:5000");
    }
}
