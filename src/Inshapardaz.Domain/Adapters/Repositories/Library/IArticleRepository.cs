using Inshapardaz.Domain.Models.Library;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Adapters.Repositories.Library
{
    public interface IIssueArticleRepository
    {
        Task<IEnumerable<IssueArticleModel>> GetArticlesByIssue(int libraryId, int periodicalId, int volumeNumber, int issueNumber, CancellationToken cancellationToken);

        Task<IssueArticleModel> GetArticle(int libraryId, int periodicalId, int volumeNumber, int issueNumber,int sequenceNumber, CancellationToken cancellationToken);

        Task<IssueArticleModel> AddArticle(int libraryId, int peridicalId, int volumeNumber, int issueNumber, IssueArticleModel article, CancellationToken cancellationToken);

        Task UpdateArticle(int libraryId, int periodicalId, int volumeNumber, int issueNumber, IssueArticleModel article, CancellationToken cancellationToken);

        Task<IEnumerable<ArticleContentModel>> GetArticleContents(int libraryId, int periodicalId, int volumeNumber, int issueNumber, int sequenceNumber, CancellationToken cancellationToken);

        Task DeleteArticle(int libraryId, int periodicalId, int volumeNumber, int issueNumber, int sequenceNumber, CancellationToken cancellationToken);

        Task<ArticleContentModel> GetArticleContentById(int libraryId, int periodicalId, int volumeNumber, int issueNumber, int sequenceNumber, string language, CancellationToken cancellationToken);

        Task<ArticleContentModel> AddArticleContent(int libraryId, int periodicalId, int volumeNumber, int issueNumber, int sequenceNumber, string language, string content, CancellationToken cancellationToken);

        Task<ArticleContentModel> UpdateArticleContent(int libraryId, int periodicalId, int volumeNumber, int issueNumber, int sequenceNumber, string language, string content, CancellationToken cancellationToken);

        Task<ArticleContentModel> GetArticleContent(int libraryId, int periodicalId, int volumeNumber, int issueNumber, int sequenceNumber, string language, CancellationToken cancellationToken);

        Task DeleteArticleContent(int libraryId, int periodicalId, int volumeNumber, int issueNumber, int sequenceNumber, CancellationToken cancellationToken);
        Task<IssueArticleModel> UpdateWriterAssignment(int libraryId, int periodicalId, int volumeNumber, int issueNumber, int sequenceNumber, int? accountId, CancellationToken cancellationToken);
        Task<IssueArticleModel> UpdateReviewerAssignment(int libraryId, int periodicalId, int volumeNumber, int issueNumber, int sequenceNumber, int? accountId, CancellationToken cancellationToken);
        Task UpdateArticleSequence(int libraryId, int periodicalId, int volumeNumber, int issueNumber, IEnumerable<IssueArticleModel> articles, CancellationToken cancellationToken);
    }
}
