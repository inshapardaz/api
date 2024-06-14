using Inshapardaz.Domain.Models;
using System;
using System.Collections.Generic;

namespace Inshapardaz.Api.Tests.Framework.Helpers
{
    public static class MiscExtentions
    {
        public static void ForEach<T>(this IEnumerable<T> enumeration, Action<T> action)
        {
            foreach (T item in enumeration)
            {
                action(item);
            }
        }

        public static string GetExtentionForMimeType(this string mimeType)
        {
            switch (mimeType.ToLower())
            {
                case MimeTypes.Markdown: 
                    return ".md";
                case MimeTypes.Pdf:
                    return ".pdf";
                case MimeTypes.Epub:
                    return ".pdf";
                case MimeTypes.Html:
                    return ".html";
                case MimeTypes.Json:
                    return ".pdf";
                case MimeTypes.MsWord:
                    return ".docx";
                case MimeTypes.Text:
                    return ".txt";
                default:
                    return string.Empty;
            }
        }
    }
}
