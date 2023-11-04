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
        public UpdateArticleContentRequest(int libraryId, int articleId, string content, string language)
            : base(libraryId)
        {
            ArticleId = articleId;
            Content = content;
            Language = language;
        }

        public int ArticleId { get; }
        public string Language { get; set; }
        public string Content { get; set; }

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
        private readonly IFileStorage _fileStorage;
        private readonly IFileRepository _fileRepository;
        private readonly ILibraryRepository _libraryRepository;

        public UpdateArticleContentRequestHandler(IArticleRepository articleRepository, IFileStorage fileStorage,
                                                  IFileRepository fileRepository, ILibraryRepository libraryRepository)
        {
            _articleRepository = articleRepository;
            _fileStorage = fileStorage;
            _fileRepository = fileRepository;
            _libraryRepository = libraryRepository;
        }

        public override async Task<UpdateArticleContentRequest> HandleAsync(UpdateArticleContentRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            var article = await _articleRepository.GetArticle(command.LibraryId, command.ArticleId, cancellationToken);

            if (article == null)
            {
                throw new BadRequestException();
            }

            if (string.IsNullOrWhiteSpace(command.Language))
            {
                var library = await _libraryRepository.GetLibraryById(command.LibraryId, cancellationToken);
                if (library == null)
                {
                    throw new BadRequestException();
                }

                command.Language = library.Language;
            }

            var content = await _articleRepository.GetArticleContent(command.LibraryId, command.ArticleId, command.Language, cancellationToken);

            if (content == null)
            {
                command.Result.Content = await _articleRepository.AddArticleContent(
                    command.LibraryId,
                    command.ArticleId,
                    command.Language,
                    command.Content,
                    cancellationToken);
                command.Result.HasAddedNew = true;
            }
            else
            {
                command.Result.Content = await _articleRepository.UpdateArticleContent(command.LibraryId,
                                                        command.ArticleId,
                                                        command.Language,
                                                        command.Content,
                                                        cancellationToken);
            }

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
