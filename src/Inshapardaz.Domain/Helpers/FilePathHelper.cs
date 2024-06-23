using DocumentFormat.OpenXml.Drawing.Charts;
using Microsoft.Extensions.DependencyModel;
using System;
using System.IO;
using System.Net;

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

    // periodical image
    public static string GetPeriodicalImageFileName(string fileName) => $"title{Path.GetExtension(fileName)}";

    public static string GetPeriodicalImageFilePath(int periodicalId, string fileName) => $"periodicals/{periodicalId}/{fileName}";

    // Issue

    public static string GetPeriodicalIssueImageFileName(string fileName) => $"issue-image{Path.GetExtension(fileName)}";

    public static string GetPeriodicalIssueImageFilePath(int periodicalId, int volumeNumber, int issueNumber, string fileName) 
        => $"periodicals/{periodicalId}/volumes/{volumeNumber}/issues/{issueNumber}/files/{fileName}";

    public static string GetIssueContentFileName(string fileName) => $"{Guid.NewGuid().ToString("N")}{Path.GetExtension(fileName)}";

    public static string GetIssueContentPath(int periodicalId, int volumeNumber, int issueNumber, string fileName)
         => $"periodicals/{periodicalId}/volumes/{volumeNumber}/issues/{issueNumber}/files/{fileName}";

    public static string IssuePageContentFileName => $"page-content-{Guid.NewGuid().ToString("N")}.md";

    public static string GetIssuePageContentPath(int periodicalId, int volumeNumber, int issueNumber, string fileName)
        => $"periodicals/{periodicalId}/volumes/{volumeNumber}/issues/{issueNumber}/pages/{fileName}";

    public static string GetIssuePageFileName(string fileName) => $"page-image-{Guid.NewGuid().ToString("N")}{Path.GetExtension(fileName)}";

    public static string GetIssuePageFilePath(int periodicalId, int volumeNumber, int issueNumber, string fileName) 
        => $"periodicals/{periodicalId}/volumes/{volumeNumber}/issues/{issueNumber}/{fileName}";
    // Issue Article

    public static string GetIssueArticleContentFileName(string language) => $"article-{language}.md";

    public static string GetIssueArticleContentPath(int periodicalId, int volumeNumber, int issueNumber, long articleId, string fileName) => $"periodicals/{periodicalId}/volumes/{volumeNumber}/issues/{issueNumber}/articles/{articleId}/{fileName}";
}
