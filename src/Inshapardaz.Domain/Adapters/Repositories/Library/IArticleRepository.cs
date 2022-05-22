using Inshapardaz.Domain.Models.Library;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Adapters.Repositories.Library
{
    public interface IArticleRepository
    {
        Task<IEnumerable<ArticleModel>> GetArticlesByIssue(int libraryId, int periodicalId, int volumeNumber, int issueNumber, CancellationToken cancellationToken);

        Task<ArticleModel> GetArticleById(int libraryId, int periodicalId, int volumeNumber, int issueNumber,int articleId, CancellationToken cancellationToken);

        Task<ArticleModel> AddArticle(int libraryId, int peridicalId, int issueId, ArticleModel article, CancellationToken cancellationToken);

        Task UpdateArticle(int libraryId, int periodicalId, int volumeNumber, int issueNumber, ArticleModel article, CancellationToken cancellationToken);

        Task<IEnumerable<ArticleContentModel>> GetArticleContents(int libraryId, int periodicalId, int volumeNumber, int issueNumber, int articleId, CancellationToken cancellationToken);

        Task DeleteArticle(int libraryId, int periodicalId, int volumeNumber, int issueNumber, int articleId, CancellationToken cancellationToken);

        Task<ArticleContentModel> GetArticleContentById(int libraryId, int periodicalId, int volumeNumber, int issueNumber, int articleId, string language, string mimeType, CancellationToken cancellationToken);

        Task<ArticleContentModel> AddArticleContent(int libraryId, ArticleContentModel issueContent, CancellationToken cancellationToken);

        Task<string> GetArticleContentUrl(int libraryId, int periodicalId, int volumeNumber, int issueNumber, int articleId, string language, string mimeType, CancellationToken cancellationToken);

        Task<ArticleContentModel> GetArticleContent(int libraryId, int periodicalId, int volumeNumber, int issueNumber, int articleId, string language, string mimeType, CancellationToken cancellationToken);

        Task DeleteArticleContentById(int libraryId, int periodicalId, int volumeNumber, int issueNumber, int articleId, CancellationToken cancellationToken);
    }
}
