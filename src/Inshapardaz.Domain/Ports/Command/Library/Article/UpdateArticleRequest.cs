using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Handlers.Library;
using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Domain.Ports.Command;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Brighter;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Handlers.Library.Article
{
    public class UpdateArticleRequest : LibraryBaseCommand
    {
        public UpdateArticleRequest(int libraryId, ArticleModel article)
            : base(libraryId)
        {
            Article = article;
        }

        public RequestResult Result { get; set; } = new RequestResult();
        public ArticleModel Article { get; }
        public int? AccountId { get; set; }

        public class RequestResult
        {
            public ArticleModel Article { get; set; }

            public bool HasAddedNew { get; set; }
        }
    }

    public class UpdateArticleRequestHandler : RequestHandlerAsync<UpdateArticleRequest>
    {
        private readonly IArticleRepository _articleRepository;
        private readonly IAuthorRepository _authorRepository;
        private readonly ICategoryRepository _categoryRepository;

        public UpdateArticleRequestHandler(IArticleRepository articleRepository, IAuthorRepository authorRepository, ICategoryRepository categoryRepository)
        {
            _articleRepository = articleRepository;
            _authorRepository = authorRepository;
            _categoryRepository = categoryRepository;
        }

        [LibraryAuthorize(1, Role.LibraryAdmin, Role.Writer)]
        public override async Task<UpdateArticleRequest> HandleAsync(UpdateArticleRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            IEnumerable<AuthorModel> authors = null;
            if (command.Article.Authors != null && command.Article.Authors.Any())
            {
                authors = await _authorRepository.GetAuthorByIds(command.LibraryId, command.Article.Authors.Select(a => a.Id), cancellationToken);
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
                categories = await _categoryRepository.GetCategoriesByIds(command.LibraryId, command.Article.Categories.Select(c => c.Id), cancellationToken);
                if (categories.Count() != command.Article.Categories.Count())
                {
                    throw new BadRequestException();
                }
            }


            var result = await _articleRepository.GetArticle(command.LibraryId, command.Article.Id, cancellationToken);

            if (result == null)
            {
                var article = command.Article;
                article.Id = default;
                command.Result.Article = await _articleRepository.AddArticle(command.LibraryId, article, command.AccountId, cancellationToken);
                command.Result.HasAddedNew = true;
            }
            else
            {
                command.Result.Article = await _articleRepository.UpdateArticle(command.LibraryId, command.Article, cancellationToken);
            }

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
