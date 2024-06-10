using System;
using System.IO;

namespace Inshapardaz.Domain.Helpers;

internal static class FilePathHelper
{
    public static string BookChapterContentFileName => $"chapter-{Guid.NewGuid().ToString("N")}.md";

    public static string GetBookChapterContentPath(int bookId, string fileName) => $"books/{bookId}/chapters/{fileName}";

    public static string BookPageContentFileName => $"page-content-{Guid.NewGuid().ToString("N")}.md";

    public static string GetBookPageContentPath(int bookId, string fileName) => $"books/{bookId}/pages/{fileName}";

    internal static string GetBookPageFileName(string fileName) => $"page-image-{Guid.NewGuid().ToString("N")}{Path.GetExtension(fileName)}";

    internal static string GetBookPageFilePath(int bookId, string fileName) => $"books/{bookId}/pages/{fileName}";

}
