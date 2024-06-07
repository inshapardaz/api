using System;
using System.IO;

namespace Inshapardaz.Domain.Helpers
{
    internal static class FilePathHelper
    {
        public static string BookPageContentFileName => $"{Guid.NewGuid().ToString("N")}.md";

        public static string GetBookPageContentPath(int bookId, string fileName) => $"books/{bookId}/pages/{fileName}";

        internal static string GetBookPageFilePath(int bookId, string fileName) => $"books/{bookId}/pages/{fileName}";

        internal static string GetBookPageFileName(string fileName) => $"{Guid.NewGuid().ToString("N")}{Path.GetExtension(fileName)}";
    }
}
