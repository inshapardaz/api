﻿using Microsoft.Extensions.DependencyModel;
using System;
using System.IO;

namespace Inshapardaz.Domain.Helpers;

public static class FilePathHelper
{
    // author
    public static string GetAuthorImageFileName(string fileName) => $"image{Path.GetExtension(fileName)}";

    public static string GetAuthorImagePath(int libararyId, int authorId, string fileName) => $"{libararyId}/authors/{authorId}/{fileName}";
    
    // series
    public static string GetSeriesImageFileName(string fileName) => $"image{Path.GetExtension(fileName)}";

    public static string GetSeriesImagePath(int libararyId, int seriesId, string fileName) => $"{libararyId}/series/{seriesId}/{fileName}";

    // book image
    internal static string GetBookImageFileName(string fileName) => $"title{Path.GetExtension(fileName)}";

    internal static string GetBookImageFilePath(int libararyId, int bookId, string fileName) => $"{libararyId}/books/{bookId}/{fileName}";

    // book content
    public static string GetBookContentFileName(string fileName) => $"{Guid.NewGuid().ToString("N")}{Path.GetExtension(fileName)}";

    public static string GetBookContentPath(int libararyId, int bookId, string fileName) => $"{libararyId}/books/{bookId}/files/{fileName}";

    // book chapter content
    public static string BookChapterContentFileName => $"chapter-{Guid.NewGuid().ToString("N")}.md";

    public static string GetBookChapterContentPath(int libararyId, int bookId, string fileName) => $"{libararyId}/books/{bookId}/chapters/{fileName}";

    // Book Page
    public static string BookPageContentFileName => $"page-content-{Guid.NewGuid().ToString("N")}.md";

    public static string GetBookPageContentPath(int libararyId, int bookId, string fileName) => $"{libararyId}/books/{bookId}/pages/{fileName}";

    internal static string GetBookPageFileName(string fileName) => $"page-image-{Guid.NewGuid().ToString("N")}{Path.GetExtension(fileName)}";

    internal static string GetBookPageFilePath(int libararyId, int bookId, string fileName) => $"{libararyId}/books/{bookId}/pages/{fileName}";

    // Article
    public static string GetArticleContentFileName(string language) => $"article-{language}.md";

    public static string GetArticleContentPath(int libararyId, long articleId, string fileName) => $"{libararyId}/articles/{articleId}/{fileName}";

    public static string GetArticleImageFileName(string fileName) => $"article-image{Path.GetExtension(fileName)}";

    public static string GetArticleImagePath(int libararyId, long articleId, string fileName) => $"{libararyId}/articles/{articleId}/{fileName}";

}
