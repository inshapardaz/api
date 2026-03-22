using Paramore.Darker;
using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Domain.Ports.Query.File;

namespace Inshapardaz.Domain.Ports.Query.Library.Article;

public class GetArticleContentQuery(int libraryId, int articleId, string language)
    : LibraryBaseQuery<ArticleContentModel>(libraryId)
{
    public int ArticleId { get; } = articleId;
    public string Language { get; set; } = language;
}

public class GetArticleContentQueryHandler(
    ILibraryRepository libraryRepository,
    IArticleRepository articleRepository,
    IQueryProcessor queryProcessor)
    : QueryHandlerAsync<GetArticleContentQuery, ArticleContentModel>
{
    [LibraryAuthorize(1)]
    public override async Task<ArticleContentModel> ExecuteAsync(GetArticleContentQuery command, CancellationToken cancellationToken = new CancellationToken())
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
        var articleContent = await articleRepository.GetArticleContent(command.LibraryId, command.ArticleId, command.Language, cancellationToken);

        if (articleContent is not null && articleContent.FileId.HasValue)
        {
            articleContent.Text = await queryProcessor.ExecuteAsync(new GetTextFileQuery(articleContent.FileId.Value), cancellationToken);
        }
        return articleContent;
    }
}
