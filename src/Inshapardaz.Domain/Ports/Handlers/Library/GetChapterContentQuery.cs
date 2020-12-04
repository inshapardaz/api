using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Models.Handlers.Library;
using Inshapardaz.Domain.Repositories;
using Inshapardaz.Domain.Helpers;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Darker;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Adapters;

namespace Inshapardaz.Domain.Models.Library
{
    public class GetChapterContentQuery : LibraryBaseQuery<ChapterContentModel>
    {
        public GetChapterContentQuery(int libraryId, int bookId, int chapterId, string language, string mimeType, int? accountId)
            : base(libraryId)
        {
            BookId = bookId;
            ChapterId = chapterId;
            MimeType = mimeType;
            AccountId = accountId;
            Language = language;
        }

        public int BookId { get; set; }

        public int ChapterId { get; }

        public string MimeType { get; set; }
        public int? AccountId { get; }
        public string Language { get; set; }
    }

    public class GetChapterContentQueryHandler : QueryHandlerAsync<GetChapterContentQuery, ChapterContentModel>
    {
        private readonly ILibraryRepository _libraryRepository;
        private readonly IBookRepository _bookRepository;
        private readonly IChapterRepository _chapterRepository;
        private readonly IFileRepository _fileRepository;

        public GetChapterContentQueryHandler(ILibraryRepository libraryRepository, IBookRepository bookRepository, IChapterRepository chapterRepository, IFileRepository fileRepository)
        {
            _libraryRepository = libraryRepository;
            _bookRepository = bookRepository;
            _chapterRepository = chapterRepository;
            _fileRepository = fileRepository;
        }

        public override async Task<ChapterContentModel> ExecuteAsync(GetChapterContentQuery command, CancellationToken cancellationToken = new CancellationToken())
        {
            var book = await _bookRepository.GetBookById(command.LibraryId, command.BookId, null, cancellationToken);
            if (book == null)
            {
                throw new NotFoundException();
            }

            if (!book.IsPublic && !command.AccountId.HasValue)
            {
                throw new UnauthorizedException();
            }

            if (string.IsNullOrWhiteSpace(command.Language))
            {
                var library = await _libraryRepository.GetLibraryById(command.LibraryId, cancellationToken);
                if (library == null)
                {
                    throw new BadRequestException();
                }

                command.Language = library.Language;
            }

            var chapterContent = await _chapterRepository.GetChapterContent(command.LibraryId, command.BookId, command.ChapterId, command.Language, command.MimeType, cancellationToken);
            if (chapterContent != null)
            {
                if (command.AccountId.HasValue)
                {
                    await _bookRepository.AddRecentBook(command.LibraryId, command.AccountId.Value, command.BookId, cancellationToken);
                }

                if (book.IsPublic)
                {
                    chapterContent.ContentUrl = await ImageHelper.TryConvertToPublicFile(chapterContent.FileId, _fileRepository, cancellationToken);
                }
            }

            return chapterContent;
        }
    }
}
