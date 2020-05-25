using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Domain.Ports.Handlers.Library;
using Inshapardaz.Domain.Repositories;
using Inshapardaz.Domain.Helpers;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Darker;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Library
{
    public class GetChapterContentQuery : LibraryBaseQuery<ChapterContentModel>
    {
        public GetChapterContentQuery(int libraryId, int bookId, int chapterId, string language, string mimeType, Guid userId)
            : base(libraryId)
        {
            UserId = userId;
            BookId = bookId;
            ChapterId = chapterId;
            MimeType = mimeType;
            Language = language;
        }

        public int BookId { get; set; }

        public int ChapterId { get; }

        public Guid UserId { get; set; }

        public string MimeType { get; set; }
        public string Language { get; set; }
    }

    public class GetChapterContentQueryHandler : QueryHandlerAsync<GetChapterContentQuery, ChapterContentModel>
    {
        private readonly IBookRepository _bookRepository;
        private readonly IChapterRepository _chapterRepository;
        private readonly IFileStorage _fileStorage;
        private readonly IFileRepository _fileRepository;

        public GetChapterContentQueryHandler(IBookRepository bookRepository, IChapterRepository chapterRepository,
                                             IFileStorage fileStorage, IFileRepository fileRepository)
        {
            _bookRepository = bookRepository;
            _chapterRepository = chapterRepository;
            _fileStorage = fileStorage;
            _fileRepository = fileRepository;
        }

        public override async Task<ChapterContentModel> ExecuteAsync(GetChapterContentQuery command, CancellationToken cancellationToken = new CancellationToken())
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

            var chapterContent = await _chapterRepository.GetChapterContent(command.LibraryId, command.BookId, command.ChapterId, command.Language, command.MimeType, cancellationToken);
            if (chapterContent != null)
            {
                if (command.UserId != Guid.Empty)
                {
                    await _bookRepository.AddRecentBook(command.LibraryId, command.UserId, command.BookId, cancellationToken);
                }

                if (book.IsPublic)
                {
                    chapterContent.ContentUrl = await ImageHelper.TryConvertToPublicImage(chapterContent.FileId, _fileRepository, cancellationToken);
                }
            }

            return chapterContent;
        }
    }
}
