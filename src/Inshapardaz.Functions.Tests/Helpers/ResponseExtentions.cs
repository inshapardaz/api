using FluentAssertions.Numeric;
using FluentAssertions.Primitives;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Inshapardaz.Functions.Tests.Helpers
{
    public static class ResponseExtentions
    {
        public static void BeOk(this NullableNumericAssertions<int> assertion)
        {
            assertion.Be((int)HttpStatusCode.OK);
        }

        public static void BeOk(this ObjectAssertions assertion)
        {
            assertion.BeOfType<OkResult>();
        }

        public static void BeCreated(this ObjectAssertions assertion)
        {
            assertion.BeOfType<CreatedResult>();
        }

        public static void BeNoContent(this NumericAssertions<int> assertion)
        {
            assertion.Be((int)HttpStatusCode.NoContent);
        }

        public static void BeBadRequest(this NumericAssertions<int> assertion)
        {
            assertion.Be((int)HttpStatusCode.BadRequest);
        }

        public static void BeNotFound(this NumericAssertions<int> assertion)
        {
            assertion.Be((int)HttpStatusCode.NotFound);
        }

        public static void BeInternalServerError(this NullableNumericAssertions<int> assertion)
        {
            assertion.Be((int)HttpStatusCode.InternalServerError);
        }

        public static void BeUnauthorized(this ObjectAssertions assertion)
        {
            assertion.BeOfType<UnauthorizedResult>();
        }

        public static void BeForbidden(this ObjectAssertions assertion)
        {
            assertion.BeOfType<ForbidResult>();
        }
    }
}
