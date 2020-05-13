using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Inshapardaz.Functions.Tests.Asserts
{
    public static class ResponseAsserts
    {
        public static void ShouldBeOk(this ObjectResult result)
        {
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(200);
        }

        public static void ShouldBeOk(this StatusCodeResult result)
        {
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(200);
        }

        public static void ShouldBeNoContent(this StatusCodeResult result)
        {
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(204);
        }

        public static void ShouldBeCreated(this ObjectResult result)
        {
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(201);
        }

        public static void ShouldBeNotFound(this StatusCodeResult result)
        {
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(404);
        }

        public static void ShouldBeUnauthorized(this ActionResult result)
        {
            result.Should().BeOfType<UnauthorizedResult>();
        }

        public static void ShouldBeForbidden(this ActionResult result)
        {
            result.Should().BeOfType<ForbidResult>();
        }

        public static void ShouldBeBadRequest(this BadRequestResult result)
        {
            result.Should().NotBeNull();
        }
    }
}
