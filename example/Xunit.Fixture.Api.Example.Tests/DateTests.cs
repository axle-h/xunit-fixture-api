using System;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit.Abstractions;
using Xunit.Fixture.Api.Example.Models;
using Xunit.Fixture.Api.Extensions;

namespace Xunit.Fixture.Api.Example.Tests
{
    public class DateTests : TestBase
    {
        public DateTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public Task When_getting_date() =>
            GivenDateApiTestFixture()
               .WhenGetting("date")
               .ShouldReturnSuccessfulStatus()
               .ShouldReturnJson<DateDto>(x => x.UtcNow.Should().BeCloseTo(DateTimeOffset.UtcNow, 1000))
               .RunAsync();
    }
}
