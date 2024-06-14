using Microsoft.Extensions.DependencyModel;
using System;
using System.IO;

namespace Inshapardaz.Domain.Helpers;

public static class FilePathHelper
{
    // author
    public static string GetAuthorImageFileName(string fileName) => $"image{Path.GetExtension(fileName)}";

    public static string GetAuthorImagePath(int authorId, string fileName) => $"authors/{authorId}/{fileName}";
    
    // series
    public static string GetSeriesImageFileName(string fileName) => $"image{Path.GetExtension(fileName)}";

    public static string GetSeriesImagePath(int seriesId, string fileName) => $"/series/{seriesId}/{fileName}";

    // book image
    public static string GetBookImageFileName(string fileName) => $"title{Path.GetExtension(fileName)}";

    public static string GetBookImageFilePath(int bookId, string fileName) => $"books/{bookId}/{fileName}";

    // book content
    public static string GetBookContentFileName(string fileName) => $"{Guid.NewGuid().ToString("N")}{Path.GetExtension(fileName)}";

    public static string GetBookContentPath(int bookId, string fileName) => $"books/{bookId}/files/{fileName}";

    // book chapter content
    public static string BookChapterContentFileName => $"chapter-{Guid.NewGuid().ToString("N")}.md";

    public static string GetBookChapterContentPath(int bookId, string fileName) => $"books/{bookId}/chapters/{fileName}";

    // Book Page
    public static string BookPageContentFileName => $"page-content-{Guid.NewGuid().ToString("N")}.md";

    public static string GetBookPageContentPath(int bookId, string fileName) => $"books/{bookId}/pages/{fileName}";

    public static string GetBookPageFileName(string fileName) => $"page-image-{Guid.NewGuid().ToString("N")}{Path.GetExtension(fileName)}";

    public static string GetBookPageFilePath(int bookId, string fileName) => $"books/{bookId}/pages/{fileName}";

    // Article
    public static string GetArticleContentFileName(string language) => $"article-{language}.md";

    public static string GetArticleContentPath(long articleId, string fileName) => $"articles/{articleId}/{fileName}";

    public static string GetArticleImageFileName(string fileName) => $"article-image{Path.GetExtension(fileName)}";

    public static string GetArticleImagePath(long articleId, string fileName) => $"articles/{articleId}/{fileName}";

}
