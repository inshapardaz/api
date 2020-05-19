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
        public DeleteChapterContentRequest(ClaimsPrincipal claims, int libraryId, int bookId, int chapterId, string mimeType, Guid userId)
            : base(claims, libraryId, bookId, userId)
        {
            ChapterId = chapterId;
            MimeType = mimeType;
        }

        public int ChapterId { get; }

        public string MimeType { get; }
    }

    public class DeleteChapterContentRequestHandler : RequestHandlerAsync<DeleteChapterContentRequest>
    {
        private readonly IChapterRepository _chapterRepository;
        private readonly IFileStorage _fileStorage;

        public DeleteChapterContentRequestHandler(IChapterRepository chapterRepository, IFileStorage fileStorage)
        {
            _chapterRepository = chapterRepository;
            _fileStorage = fileStorage;
        }

        [Authorise(step: 1, HandlerTiming.Before)]
        public override async Task<DeleteChapterContentRequest> HandleAsync(DeleteChapterContentRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            var contentUrl = await _chapterRepository.GetChapterContentUrl(command.LibraryId, command.BookId, command.ChapterId, command.MimeType, cancellationToken);

            if (!string.IsNullOrWhiteSpace(contentUrl))
            {
                await _fileStorage.TryDeleteFile(contentUrl, cancellationToken);
            }

            await _chapterRepository.DeleteChapterContentById(command.LibraryId, command.BookId, command.ChapterId, command.MimeType, cancellationToken);

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
