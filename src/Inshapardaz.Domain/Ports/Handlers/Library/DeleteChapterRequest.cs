using Inshapardaz.Domain.Repositories;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Brighter;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Models.Library
{
    public class DeleteChapterRequest : BookRequest
    {
        public DeleteChapterRequest(int libraryId, int bookId, int chapterNumber)
            : base(libraryId, bookId)
        {
            ChapterNumber = chapterNumber;
        }

        public int ChapterNumber { get; }
    }

    public class DeleteChapterRequestHandler : RequestHandlerAsync<DeleteChapterRequest>
    {
        private readonly IChapterRepository _chapterRepository;
        private readonly IFileStorage _fileStorage;

        public DeleteChapterRequestHandler(IChapterRepository chapterRepository, IFileStorage fileStorage)
        {
            _chapterRepository = chapterRepository;
            _fileStorage = fileStorage;
        }

        public override async Task<DeleteChapterRequest> HandleAsync(DeleteChapterRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            //TODO:  support multiple
            var filePath = await _chapterRepository.GetChapterContentUrl(command.LibraryId, command.BookId, command.ChapterNumber, "", "", cancellationToken);

            await _chapterRepository.DeleteChapter(command.LibraryId, command.BookId, command.ChapterNumber, cancellationToken);

            if (!string.IsNullOrWhiteSpace(filePath))
                await _fileStorage.TryDeleteFile(filePath, cancellationToken);

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
