using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Models.Handlers.Library;
using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Domain.Repositories;
using Paramore.Brighter;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Handlers.Library.Article
{
    public class UpdateArticleContentRequest : LibraryBaseCommand
    {
        public UpdateArticleContentRequest(int libraryId)
            : base(libraryId)
        {
        }

        public ArticleContentModel Content { get; set; }


        public RequestResult Result { get; set; } = new RequestResult();

        public class RequestResult
        {
            public ArticleContentModel Content { get; set; }

            public bool HasAddedNew { get; set; }
        }
    }

    public class UpdateArticleContentRequestHandler : RequestHandlerAsync<UpdateArticleContentRequest>
    {
        private readonly IArticleRepository _articleRepository;
        private readonly ILibraryRepository _libraryRepository;

        public UpdateArticleContentRequestHandler(IArticleRepository articleRepository, ILibraryRepository libraryRepository)
        {
            _articleRepository = articleRepository;
            _libraryRepository = libraryRepository;
        }

        public override async Task<UpdateArticleContentRequest> HandleAsync(UpdateArticleContentRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            var article = await _articleRepository.GetArticle(command.LibraryId, command.Content.ArticleId, cancellationToken);

            if (article == null)
            {
                throw new BadRequestException();
            }

            if (string.IsNullOrWhiteSpace(command.Content.Language))
            {
                var library = await _libraryRepository.GetLibraryById(command.LibraryId, cancellationToken);
                if (library == null)
                {
                    throw new BadRequestException();
                }

                command.Content.Language = library.Language;
            }

            var content = await _articleRepository.GetArticleContent(command.LibraryId, command.Content.ArticleId, command.Content.Language, cancellationToken);

            if (content == null)
            {
                command.Result.Content = await _articleRepository.AddArticleContent(
                    command.LibraryId,
                    command.Content,
                    cancellationToken);
                command.Result.HasAddedNew = true;
            }
            else
            {
                command.Result.Content = await _articleRepository.UpdateArticleContent(command.LibraryId,
                                                        command.Content,
                                                        cancellationToken);
            }

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
