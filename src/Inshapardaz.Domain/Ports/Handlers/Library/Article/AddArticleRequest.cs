using DocumentFormat.OpenXml.Spreadsheet;
using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Models.Handlers.Library;
using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Brighter;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Handlers.Library.Article
{
    public class AddArticleRequest : LibraryBaseCommand
    {
        public AddArticleRequest(int libraryId, ArticleModel article)
            : base(libraryId)
        {
            Article = article;
        }

        public int? AccountId { get; set; }

        public ArticleModel Result { get; set; }
        public ArticleModel Article { get; }
    }

    public class AddArticleRequestHandler : RequestHandlerAsync<AddArticleRequest>
    {
        private readonly IArticleRepository _articleRepository;
        private readonly IAuthorRepository _authorRepository;
        private readonly ICategoryRepository _categoryRepository;

        public AddArticleRequestHandler(IArticleRepository articleRepository, 
            IAuthorRepository authorRepository,
            ICategoryRepository categoryRepository)
        {
            _articleRepository = articleRepository;
            _authorRepository = authorRepository;
            _categoryRepository = categoryRepository;
        }

        [UseLibraryCheck(1, HandlerTiming.Before)]
        public override async Task<AddArticleRequest> HandleAsync(AddArticleRequest command, CancellationToken cancellationToken = new CancellationToken())
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

            if (authors == null || authors.FirstOrDefault() == null)
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

            command.Result = await _articleRepository.AddArticle(command.LibraryId, command.Article, command.AccountId, cancellationToken);

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
