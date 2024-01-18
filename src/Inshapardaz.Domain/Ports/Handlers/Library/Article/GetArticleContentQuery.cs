using Inshapardaz.Domain.Models.Handlers.Library;
using Inshapardaz.Domain.Repositories;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Darker;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Models.Library;

namespace Inshapardaz.Domain.Ports.Handlers.Library.Article
{
    public class GetArticleContentQuery : LibraryBaseQuery<ArticleContentModel>
    {
        public GetArticleContentQuery(int libraryId, int articleId, string language)
            : base(libraryId)
        {
            ArticleId = articleId;
            Language = language;
        }

        public int ArticleId { get; }
        public string Language { get; set; }
    }

    public class GetArticleContentQueryHandler : QueryHandlerAsync<GetArticleContentQuery, ArticleContentModel>
    {
        private readonly ILibraryRepository _libraryRepository;
        private readonly IArticleRepository _articleRepository;

        public GetArticleContentQueryHandler(ILibraryRepository libraryRepository, IArticleRepository articleRepository)
        {
            _libraryRepository = libraryRepository;
            _articleRepository = articleRepository;
        }

        public override async Task<ArticleContentModel> ExecuteAsync(GetArticleContentQuery command, CancellationToken cancellationToken = new CancellationToken())
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
            return await _articleRepository.GetArticleContent(command.LibraryId, command.ArticleId, command.Language, cancellationToken);
        }
    }
}
