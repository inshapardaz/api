namespace Inshapardaz.Domain.Models
{
    public static class MimeTypes
    {
        public const string Json = "application/json";

        public const string Markdown = "text/markdown";

        public const string Pdf = "application/pdf";

        public const string Jpg = "image/jpeg";

        public const string Text = "text/plain";

        public const string Html = "text/html";

        public const string MsWord = "application/msword";

        public const string Epub = "application/epub+zip";

        public const string Zip = "application/zip";

        public const string CompressedFile = "application/x-zip-compressed";

        public static string WordDoc { get; internal set; }
    }
}
