using Paramore.Darker;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Domain.Ports.Query.File;

namespace Inshapardaz.Domain.Ports.Query.Library.Periodical.Issue.Article;

public class GetIssueArticleContentQuery : LibraryBaseQuery<IssueArticleContentModel>
{
    public GetIssueArticleContentQuery(int libraryId, int periodicalId, int volumeNumber, int issueNumber, int articleId, string language)
        : base(libraryId)
    {
        PeriodicalId = periodicalId;
        VolumeNumber = volumeNumber;
        IssueNumber = issueNumber;
        ArticleId = articleId;
        Language = language;
    }

    public int PeriodicalId { get; }
    public int VolumeNumber { get; }
    public int IssueNumber { get; }
    public int ArticleId { get; }
    public string Language { get; set; }
}

public class GetArticleContentQueryHandler : QueryHandlerAsync<GetIssueArticleContentQuery, IssueArticleContentModel>
{
    private readonly ILibraryRepository _libraryRepository;
    private readonly IIssueArticleRepository _articleRepository;
    private readonly IQueryProcessor _queryProcessor;

    public GetArticleContentQueryHandler(ILibraryRepository libraryRepository, IIssueArticleRepository articleRepository, IQueryProcessor queryProcessor)
    {
        _libraryRepository = libraryRepository;
        _articleRepository = articleRepository;
        _queryProcessor = queryProcessor;
    }

    [LibraryAuthorize(1)]
    public override async Task<IssueArticleContentModel> ExecuteAsync(GetIssueArticleContentQuery command, CancellationToken cancellationToken = new CancellationToken())
    {
        if (string.IsNullOrWhiteSpace(command.Language))
        {
            var library = await _libraryRepository.GetLibraryById(command.LibraryId, cancellationToken);
            if (library == null)
            {
                throw new BadRequestException();
            }

            command.Language = library.Language;
        }
        var articleContent = await _articleRepository.GetIssueArticleContentById(command.LibraryId, command.PeriodicalId, command.VolumeNumber, command.IssueNumber, command.ArticleId, command.Language, cancellationToken);
        if (articleContent is not null && articleContent.FileId.HasValue)
        {
            articleContent.Text = await _queryProcessor.ExecuteAsync(new GetTextFileQuery(articleContent.FileId.Value), cancellationToken);
        }

        return articleContent;
    }
}
