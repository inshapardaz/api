using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Domain.Ports.Handlers.Library;
using Inshapardaz.Domain.Repositories;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Darker;
using System;
using System.Runtime.InteropServices.ComTypes;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Library
{
    public class GetChapterContentByIdQuery : LibraryBaseQuery<ChapterContentModel>
    {
        public GetChapterContentByIdQuery(int libraryId, int bookId, int chapterId, int contentId, Guid userId)
            : base(libraryId)
        {
            UserId = userId;
            BookId = bookId;
            ChapterId = chapterId;
            ContentId = contentId;
        }

        public int BookId { get; set; }

        public int ChapterId { get; }
        public int ContentId { get; }
        public Guid UserId { get; set; }
    }

    public class GetChapterContentByIdQueryHandler : QueryHandlerAsync<GetChapterContentByIdQuery, ChapterContentModel>
    {
        private readonly IBookRepository _bookRepository;
        private readonly IChapterRepository _chapterRepository;

        public GetChapterContentByIdQueryHandler(IBookRepository bookRepository, IChapterRepository chapterRepository)
        {
            _bookRepository = bookRepository;
            _chapterRepository = chapterRepository;
        }

        public override async Task<ChapterContentModel> ExecuteAsync(GetChapterContentByIdQuery command, CancellationToken cancellationToken = new CancellationToken())
        {
            var book = await _bookRepository.GetBookById(command.LibraryId, command.BookId, null, cancellationToken);
            if (book == null)
            {
                throw new NotFoundException();
            }

            if (!book.IsPublic && command.UserId == Guid.Empty)
            {
                throw new UnauthorizedException();
            }

            var chapterContent = await _chapterRepository.GetChapterContentById(command.LibraryId, command.BookId, command.ChapterId, command.ContentId, cancellationToken);
            if (chapterContent != null)
            {
                if (command.UserId != Guid.Empty)
                {
                    await _bookRepository.AddRecentBook(command.LibraryId, command.UserId, command.BookId, cancellationToken);
                }
            }

            return chapterContent;
        }
    }
}
