using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Entities.Library;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Library
{
    public class GetChapterContentRequest : BookRequest
    {
        public GetChapterContentRequest(int bookId, int chapterId)
            : base(bookId)
        {
            ChapterId = chapterId;
        }

        public int ChapterId { get; }

        public Guid UserId { get; set; }

        public ChapterContent Result { get; set; }
    }

    public class GetChapterContentRequestHandler : RequestHandlerAsync<GetChapterContentRequest>
    {
        private readonly IBookRepository _bookRepository;
        private readonly IChapterRepository _chapterRepository;

        public GetChapterContentRequestHandler(IBookRepository bookRepository, IChapterRepository chapterRepository)
        {
            _bookRepository = bookRepository;
            _chapterRepository = chapterRepository;
        }

        public override async Task<GetChapterContentRequest> HandleAsync(GetChapterContentRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            var chapters = await _chapterRepository.GetChapterContents(command.BookId, command.ChapterId, cancellationToken);
            await _bookRepository.AddRecentBook(command.UserId, command.BookId, cancellationToken);
            command.Result = chapters;
            
            return await base.HandleAsync(command, cancellationToken);
        }
    }
}

