using Microsoft.Extensions.DependencyModel;
using System;
using System.IO;

namespace Inshapardaz.Domain.Helpers;

internal static class FilePathHelper
{
    public static string BookChapterContentFileName => $"chapter-{Guid.NewGuid().ToString("N")}.md";

    public static string GetBookChapterContentPath(int libararyId, int bookId, string fileName) => $"{libararyId}/books/{bookId}/chapters/{fileName}";

    public static string BookPageContentFileName => $"page-content-{Guid.NewGuid().ToString("N")}.md";

    public static string GetBookPageContentPath(int libararyId, int bookId, string fileName) => $"{libararyId}/books/{bookId}/pages/{fileName}";

    internal static string GetBookPageFileName(string fileName) => $"page-image-{Guid.NewGuid().ToString("N")}{Path.GetExtension(fileName)}";

    internal static string GetBookPageFilePath(int libararyId, int bookId, string fileName) => $"{libararyId}/books/{bookId}/pages/{fileName}";

    public static string GetArticleContentFileName(string language) => $"article-{language}.md";

    public static string GetArticleContentPath(int libararyId, long articleId, string fileName) => $"{libararyId}/articles/{articleId}/{fileName}";

    public static string GetArticleImageFileName(string fileName) => $"article-image{Path.GetExtension(fileName)}";

    public static string GetArticleImagePath(int libararyId, long articleId, string fileName) => $"{libararyId}/articles/{articleId}/{fileName}";

}
