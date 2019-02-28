using System;
using Microsoft.AspNetCore.Mvc;
using Xunit.Fixture.Api.Example.Models;

namespace Xunit.Fixture.Api.Example.Controllers
{
    [Route("date")]
    public class DateController : Controller
    {
        [HttpGet]
        public DateDto Get() => new DateDto
                                {
                                    UtcNow = DateTimeOffset.UtcNow
                                };
    }
}