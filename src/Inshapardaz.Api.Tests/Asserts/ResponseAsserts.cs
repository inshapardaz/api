using FluentAssertions;
using System.Net;
using System.Net.Http;

namespace Inshapardaz.Api.Tests.Asserts
{
    public static class ResponseAsserts
    {
        public static void ShouldBeOk(this HttpResponseMessage result)
        {
            result.Should().NotBeNull();
            var body = result.Content.ReadAsStringAsync().Result;
            result.StatusCode.Should().Be(HttpStatusCode.OK, body);
        }

        public static void ShouldBeRedirect(this HttpResponseMessage result)
        {
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(HttpStatusCode.Redirect);
        }

        public static void ShouldBeNoContent(this HttpResponseMessage result)
        {
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }

        public static void ShouldBeCreated(this HttpResponseMessage result)
        {
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(HttpStatusCode.Created);
        }

        public static void ShouldBeNotFound(this HttpResponseMessage result)
        {
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        public static void ShouldBeUnauthorized(this HttpResponseMessage result)
        {
            result.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        public static void ShouldBeForbidden(this HttpResponseMessage result)
        {
            result.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        public static void ShouldBeBadRequest(this HttpResponseMessage result)
        {
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        public static void ShouldBeGone(this HttpResponseMessage result)
        {
            result.StatusCode.Should().Be(HttpStatusCode.Gone);
        }

        public static void ShouldBeInternalServerError(this HttpResponseMessage result)
        {
            result.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        }

        public static void ShouldBeConflict(this HttpResponseMessage result)
        {
            result.StatusCode.Should().Be(HttpStatusCode.Conflict);
        }
    }
}
