using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Brighter;
using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Models.Library
{
    public class AddChapterRequest : BookRequest
    {
        public AddChapterRequest(ClaimsPrincipal claims, int libraryId, int bookId, ChapterModel chapter, Guid? userId)
            : base(claims, libraryId, bookId, userId)
        {
            Chapter = chapter;
        }

        public ChapterModel Chapter { get; }

        public ChapterModel Result { get; set; }
    }

    public class AddChapterRequestHandler : RequestHandlerAsync<AddChapterRequest>
    {
        private readonly IChapterRepository _chapterRepository;
        private readonly IBookRepository _bookRepository;

        public AddChapterRequestHandler(IChapterRepository chapterRepository, IBookRepository bookRepository)
        {
            _chapterRepository = chapterRepository;
            _bookRepository = bookRepository;
        }

        [Authorise(step: 1, HandlerTiming.Before)]
        public override async Task<AddChapterRequest> HandleAsync(AddChapterRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            var book = await _bookRepository.GetBookById(command.LibraryId, command.BookId, command.UserId, cancellationToken);
            if (book == null)
            {
                throw new BadRequestException();
            }

            command.Result = await _chapterRepository.AddChapter(command.LibraryId, command.BookId, command.Chapter, cancellationToken);

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
