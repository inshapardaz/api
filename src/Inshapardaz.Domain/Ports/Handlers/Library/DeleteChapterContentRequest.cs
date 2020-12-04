using Inshapardaz.Domain.Repositories;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Brighter;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Models.Library
{
    public class DeleteChapterContentRequest : BookRequest
    {
        public DeleteChapterContentRequest(int libraryId, int bookId, int chapterId, string language, string mimeType)
            : base(libraryId, bookId)
        {
            ChapterId = chapterId;
            MimeType = mimeType;
            Language = language;
        }

        public int ChapterId { get; }

        public string MimeType { get; }
        public string Language { get; }
    }

    public class DeleteChapterContentRequestHandler : RequestHandlerAsync<DeleteChapterContentRequest>
    {
        private readonly IChapterRepository _chapterRepository;
        private readonly IFileRepository _fileRepository;
        private readonly IFileStorage _fileStorage;

        public DeleteChapterContentRequestHandler(IChapterRepository chapterRepository, IFileRepository fileRepository, IFileStorage fileStorage)
        {
            _chapterRepository = chapterRepository;
            _fileRepository = fileRepository;
            _fileStorage = fileStorage;
        }

        public override async Task<DeleteChapterContentRequest> HandleAsync(DeleteChapterContentRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            var content = await _chapterRepository.GetChapterContent(command.LibraryId, command.BookId, command.ChapterId, command.Language, command.MimeType, cancellationToken);

            if (content != null)
            {
                if (!string.IsNullOrWhiteSpace(content.ContentUrl))
                {
                    await _fileStorage.TryDeleteFile(content.ContentUrl, cancellationToken);
                }

                await _fileRepository.DeleteFile(content.FileId, cancellationToken);
                await _chapterRepository.DeleteChapterContentById(command.LibraryId, command.BookId, command.ChapterId, command.Language, command.MimeType, cancellationToken);
            }

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
