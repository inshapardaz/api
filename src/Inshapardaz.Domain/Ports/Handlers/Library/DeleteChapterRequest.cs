using Inshapardaz.Domain.Adapters;
using Inshapardaz.Domain.Repositories;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Brighter;
using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Models.Library
{
    public class DeleteChapterRequest : BookRequest
    {
        public DeleteChapterRequest(ClaimsPrincipal claims, int libraryId, int bookId, int chapterId, Guid? userId)
            : base(claims, libraryId, bookId, userId)
        {
            ChapterId = chapterId;
        }

        public int ChapterId { get; }
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

        [Authorise(step: 0, HandlerTiming.Before, Permission.Admin, Permission.LibraryAdmin, Permission.Writer)]
        public override async Task<DeleteChapterRequest> HandleAsync(DeleteChapterRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            //TODO:  support multiple
            var filePath = await _chapterRepository.GetChapterContentUrl(command.LibraryId, command.BookId, command.ChapterId, "", "", cancellationToken);

            await _chapterRepository.DeleteChapter(command.LibraryId, command.BookId, command.ChapterId, cancellationToken);

            if (!string.IsNullOrWhiteSpace(filePath))
                await _fileStorage.TryDeleteFile(filePath, cancellationToken);

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
