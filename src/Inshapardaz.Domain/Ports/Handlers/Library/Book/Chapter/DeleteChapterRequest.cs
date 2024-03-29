﻿using Inshapardaz.Domain.Ports.Handlers.Library.Book;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Brighter;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Handlers.Library.Book.Chapter
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

        public DeleteChapterRequestHandler(IChapterRepository chapterRepository)
        {
            _chapterRepository = chapterRepository;
        }

        public override async Task<DeleteChapterRequest> HandleAsync(DeleteChapterRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            await _chapterRepository.DeleteChapter(command.LibraryId, command.BookId, command.ChapterNumber, cancellationToken);

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
