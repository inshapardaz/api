using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Command.Library.Article;

public class UpdateArticleRequest(int libraryId, ArticleModel article) : LibraryBaseCommand(libraryId)
{
    public RequestResult Result { get; set; } = new RequestResult();
    public ArticleModel Article { get; } = article;
    public int? AccountId { get; set; }

    public class RequestResult
    {
        public ArticleModel Article { get; set; }

        public bool HasAddedNew { get; set; }
    }
}

public class UpdateArticleRequestHandler(
    IArticleRepository articleRepository,
    IAuthorRepository authorRepository,
    ICategoryRepository categoryRepository)
    : RequestHandlerAsync<UpdateArticleRequest>
{
    [LibraryAuthorize(1, Role.LibraryAdmin, Role.Writer)]
    public override async Task<UpdateArticleRequest> HandleAsync(UpdateArticleRequest command, CancellationToken cancellationToken = new CancellationToken())
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

        if (authors == null || !authors.Any())
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


        var result = await articleRepository.GetArticle(command.LibraryId, command.Article.Id, cancellationToken);

        if (result == null)
        {
            var article = command.Article;
            article.Id = default;
            command.Result.Article = await articleRepository.AddArticle(command.LibraryId, article, command.AccountId, cancellationToken);
            command.Result.HasAddedNew = true;
        }
        else
        {
            command.Result.Article = await articleRepository.UpdateArticle(command.LibraryId, command.Article, cancellationToken);
        }

        return await base.HandleAsync(command, cancellationToken);
    }
}
