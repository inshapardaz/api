using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Handlers.Library;
using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Domain.Ports.Command;
using Inshapardaz.Domain.Repositories;
using Paramore.Brighter;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Handlers.Library.Article
{
    public class AddArticleContentRequest : LibraryBaseCommand
    {
        public AddArticleContentRequest(int libraryId)
            : base(libraryId)
        {
        }

        public ArticleContentModel Content { get; set; }

        public ArticleContentModel Result { get; set; }
    }

    public class AddArticleContentRequestHandler : RequestHandlerAsync<AddArticleContentRequest>
    {
        private readonly IArticleRepository _articleRepository;
        private readonly IFileStorage _fileStorage;
        private readonly ILibraryRepository _libraryRepository;
        private readonly IFileRepository _fileRepository;

        public AddArticleContentRequestHandler(IArticleRepository articleRepository, IFileStorage fileStorage, ILibraryRepository libraryRepository, IFileRepository fileRepository)
        {
            _articleRepository = articleRepository;
            _fileStorage = fileStorage;
            _libraryRepository = libraryRepository;
            _fileRepository = fileRepository;
        }

        [LibraryAuthorize(1, Role.LibraryAdmin, Role.Writer)]
        public override async Task<AddArticleContentRequest> HandleAsync(AddArticleContentRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            if (string.IsNullOrWhiteSpace(command.Content.Language))
            {
                var library = await _libraryRepository.GetLibraryById(command.LibraryId, cancellationToken);
                if (library == null)
                {
                    throw new BadRequestException();
                }

                command.Content.Language = library.Language;
            }

            var article = await _articleRepository.GetArticle(command.LibraryId, command.Content.ArticleId, cancellationToken);

            if (article != null)
            {
                command.Result = await _articleRepository.AddArticleContent(
                    command.LibraryId,
                    command.Content,
                    cancellationToken);
            }

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
