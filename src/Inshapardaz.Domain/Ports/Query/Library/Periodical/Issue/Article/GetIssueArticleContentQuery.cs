using Paramore.Darker;
using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Domain.Ports.Query.File;

namespace Inshapardaz.Domain.Ports.Query.Library.Periodical.Issue.Article;

public class GetIssueArticleContentQuery(
    int libraryId,
    int periodicalId,
    int volumeNumber,
    int issueNumber,
    int articleId,
    string language)
    : LibraryBaseQuery<IssueArticleContentModel>(libraryId)
{
    public int PeriodicalId { get; } = periodicalId;
    public int VolumeNumber { get; } = volumeNumber;
    public int IssueNumber { get; } = issueNumber;
    public int ArticleId { get; } = articleId;
    public string Language { get; set; } = language;
}

public class GetArticleContentQueryHandler(
    ILibraryRepository libraryRepository,
    IIssueArticleRepository articleRepository,
    IQueryProcessor queryProcessor)
    : QueryHandlerAsync<GetIssueArticleContentQuery, IssueArticleContentModel>
{
    [LibraryAuthorize(1)]
    public override async Task<IssueArticleContentModel> ExecuteAsync(GetIssueArticleContentQuery command, CancellationToken cancellationToken = new CancellationToken())
    {
        if (string.IsNullOrWhiteSpace(command.Language))
        {
            var library = await libraryRepository.GetLibraryById(command.LibraryId, cancellationToken);
            if (library == null)
            {
                throw new BadRequestException();
            }

            command.Language = library.Language;
        }
        var articleContent = await articleRepository.GetIssueArticleContentById(command.LibraryId, command.PeriodicalId, command.VolumeNumber, command.IssueNumber, command.ArticleId, command.Language, cancellationToken);
        if (articleContent is not null && articleContent.FileId.HasValue)
        {
            articleContent.Text = await queryProcessor.ExecuteAsync(new GetTextFileQuery(articleContent.FileId.Value), cancellationToken);
        }

        return articleContent;
    }
}
