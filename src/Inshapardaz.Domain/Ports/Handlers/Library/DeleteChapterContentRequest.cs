using Inshapardaz.Domain.Repositories;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Brighter;
using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Library
{
    public class DeleteChapterContentRequest : BookRequest
    {
        public DeleteChapterContentRequest(ClaimsPrincipal claims, int libraryId, int bookId, int chapterId, string language, string mimeType, Guid userId)
            : base(claims, libraryId, bookId, userId)
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

        [Authorise(step: 1, HandlerTiming.Before)]
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
