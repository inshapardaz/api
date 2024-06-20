using Inshapardaz.Domain.Models.Library;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Adapters.Repositories.Library;

public interface IIssueArticleRepository
{
    Task<IEnumerable<IssueArticleModel>> GetArticlesByIssue(int libraryId, int periodicalId, int volumeNumber, int issueNumber, CancellationToken cancellationToken);

    Task<IssueArticleModel> GetArticle(int libraryId, int periodicalId, int volumeNumber, int issueNumber, int sequenceNumber, CancellationToken cancellationToken);

    Task<IssueArticleModel> AddArticle(int libraryId, int peridicalId, int volumeNumber, int issueNumber, IssueArticleModel article, CancellationToken cancellationToken);

    Task UpdateArticle(int libraryId, int periodicalId, int volumeNumber, int issueNumber, IssueArticleModel article, CancellationToken cancellationToken);

    Task<IEnumerable<IssueArticleContentModel>> GetArticleContents(int libraryId, int periodicalId, int volumeNumber, int issueNumber, int sequenceNumber, CancellationToken cancellationToken);

    Task DeleteArticle(int libraryId, int periodicalId, int volumeNumber, int issueNumber, int sequenceNumber, CancellationToken cancellationToken);

    Task<IssueArticleContentModel> GetArticleContentById(int libraryId, int periodicalId, int volumeNumber, int issueNumber, int sequenceNumber, string language, CancellationToken cancellationToken);

    Task<IssueArticleContentModel> AddArticleContent(int libraryId, IssueArticleContentModel content, CancellationToken cancellationToken);

    Task<IssueArticleContentModel> UpdateArticleContent(int libraryId, IssueArticleContentModel content, CancellationToken cancellationToken);

    Task<IssueArticleContentModel> GetArticleContent(int libraryId, IssueArticleContentModel content, CancellationToken cancellationToken);

    Task DeleteArticleContent(int libraryId, int periodicalId, int volumeNumber, int issueNumber, int sequenceNumber, CancellationToken cancellationToken);
    Task<IssueArticleModel> UpdateWriterAssignment(int libraryId, int periodicalId, int volumeNumber, int issueNumber, int sequenceNumber, int? accountId, CancellationToken cancellationToken);
    Task<IssueArticleModel> UpdateReviewerAssignment(int libraryId, int periodicalId, int volumeNumber, int issueNumber, int sequenceNumber, int? accountId, CancellationToken cancellationToken);
    Task UpdateArticleSequence(int libraryId, int periodicalId, int volumeNumber, int issueNumber, IEnumerable<IssueArticleModel> articles, CancellationToken cancellationToken);
}
