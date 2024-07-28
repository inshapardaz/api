using Inshapardaz.Domain.Models.Library;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Adapters.Repositories.Library;

public interface IIssueArticleRepository
{
    Task<IEnumerable<IssueArticleModel>> GetIssueArticlesByIssue(int libraryId, int periodicalId, int volumeNumber, int issueNumber, CancellationToken cancellationToken);

    Task<IssueArticleModel> GetIssueArticle(int libraryId, int periodicalId, int volumeNumber, int issueNumber, int sequenceNumber, CancellationToken cancellationToken);

    Task<IssueArticleModel> AddIssueArticle(int libraryId, int peridicalId, int volumeNumber, int issueNumber, IssueArticleModel issueArticle, CancellationToken cancellationToken);

    Task<IssueArticleModel> UpdateIssueArticle(int libraryId, int periodicalId, int volumeNumber, int issueNumber, IssueArticleModel issueArticle, CancellationToken cancellationToken);

    Task<IEnumerable<IssueArticleContentModel>> GetIssueArticleContents(int libraryId, int periodicalId, int volumeNumber, int issueNumber, int sequenceNumber, CancellationToken cancellationToken);

    Task DeleteIssueArticle(int libraryId, int periodicalId, int volumeNumber, int issueNumber, int sequenceNumber, CancellationToken cancellationToken);

    Task<IssueArticleContentModel> GetIssueArticleContentById(int libraryId, int periodicalId, int volumeNumber, int issueNumber, int sequenceNumber, string language, CancellationToken cancellationToken);

    Task<IssueArticleContentModel> AddIssueArticleContent(int libraryId, IssueArticleContentModel content, CancellationToken cancellationToken);

    Task<IssueArticleContentModel> UpdateIssueArticleContent(int libraryId, IssueArticleContentModel content, CancellationToken cancellationToken);

    Task<IssueArticleContentModel> GetIssueArticleContent(int libraryId, IssueArticleContentModel content, CancellationToken cancellationToken);

    Task DeleteIssueArticleContent(int libraryId, int periodicalId, int volumeNumber, int issueNumber, int sequenceNumber, CancellationToken cancellationToken);
    Task<IssueArticleModel> UpdateWriterAssignment(int libraryId, int periodicalId, int volumeNumber, int issueNumber, int sequenceNumber, int? accountId, CancellationToken cancellationToken);
    Task<IssueArticleModel> UpdateReviewerAssignment(int libraryId, int periodicalId, int volumeNumber, int issueNumber, int sequenceNumber, int? accountId, CancellationToken cancellationToken);
    Task UpdateArticleSequence(int libraryId, int periodicalId, int volumeNumber, int issueNumber, IEnumerable<IssueArticleModel> articles, CancellationToken cancellationToken);
}
