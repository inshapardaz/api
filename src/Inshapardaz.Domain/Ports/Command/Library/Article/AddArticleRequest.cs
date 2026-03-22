using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Command.Library.Article;

public class AddArticleRequest(int libraryId, ArticleModel article) : LibraryBaseCommand(libraryId)
{
    public int? AccountId { get; set; }

    public ArticleModel Result { get; set; }
    public ArticleModel Article { get; } = article;
}

public class AddArticleRequestHandler(
    IArticleRepository articleRepository,
    IAuthorRepository authorRepository,
    ICategoryRepository categoryRepository)
    : RequestHandlerAsync<AddArticleRequest>
{
    [LibraryAuthorize(1, Role.LibraryAdmin, Role.Writer)]
    public override async Task<AddArticleRequest> HandleAsync(AddArticleRequest command, CancellationToken cancellationToken = new CancellationToken())
    {
        IEnumerable<AuthorModel> authors = null;
        if (command.Article.Authors != null && command.Article.Authors.Any())
        {
            authors = await authorRepository.GetAuthorByIds(command.LibraryId, command.Article.Authors.Select(a => a.Id), cancellationToken);
            if (authors.Count() != command.Article.Authors.Count())
            {
                throw new BadRequestException();
            }
        }

        if (authors == null || authors.FirstOrDefault() == null)
        {
            throw new BadRequestException();
        }

        IEnumerable<CategoryModel> categories = null;
        if (command.Article.Categories != null && command.Article.Categories.Any())
        {
            categories = await categoryRepository.GetCategoriesByIds(command.LibraryId, command.Article.Categories.Select(c => c.Id), cancellationToken);
            if (categories.Count() != command.Article.Categories.Count())
            {
                throw new BadRequestException();
            }
        }

        command.Result = await articleRepository.AddArticle(command.LibraryId, command.Article, command.AccountId, cancellationToken);

        return await base.HandleAsync(command, cancellationToken);
    }
}
