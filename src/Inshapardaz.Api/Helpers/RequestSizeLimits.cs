namespace Inshapardaz.Api.Helpers;

// Explicit upper bounds for request/upload size (see issue #36) -- previously every limit in
// the pipeline was unbounded (Kestrel MaxRequestBodySize = null, FormOptions at int.MaxValue,
// [RequestSizeLimit(long.MaxValue)] on the bulk page upload endpoints), which combined with no
// rate limiting made the API a resource-exhaustion/DoS target.
public static class RequestSizeLimits
{
    // Default cap for ordinary requests: JSON bodies, single-file uploads (book/issue content
    // files, cover images, etc.).
    public const long Default = 200L * 1024 * 1024; // 200 MB

    // Bulk page upload endpoints (BookPageController/IssuePageController UploadPages) accept a
    // single PDF or zip containing every scanned page of a book/issue, which legitimately runs
    // much larger than any other upload in the app.
    public const long BulkPageUpload = 1024L * 1024 * 1024; // 1 GB
}
