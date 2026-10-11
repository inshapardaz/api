using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

namespace Inshapardaz.Api.Extensions;

public static class ETagExtentions
{
    // 200 with an ETag computed from the payload, or 304 when the client already holds that version
    // (If-None-Match). Meant for large lists the editors download and cache, e.g. word lists.
    public static IActionResult OkWithETag(this ControllerBase controller, object payload)
    {
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(payload)));
        var etag = new EntityTagHeaderValue($"\"{Convert.ToHexString(hash, 0, 16)}\"");

        controller.Response.Headers[HeaderNames.ETag] = etag.ToString();

        var ifNoneMatch = controller.Request.GetTypedHeaders().IfNoneMatch;
        if (ifNoneMatch.Any(t => t.Equals(EntityTagHeaderValue.Any) || t.Compare(etag, useStrongComparison: false)))
        {
            return new StatusCodeResult(StatusCodes.Status304NotModified);
        }

        return controller.Ok(payload);
    }
}
