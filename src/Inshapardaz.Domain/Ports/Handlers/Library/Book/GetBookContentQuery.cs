using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Helpers;
using Inshapardaz.Domain.Models.Handlers.Library;
using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Domain.Repositories;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Darker;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Handlers.Library.Book
{
    public class GetBookContentQuery : LibraryBaseQuery<BookContentModel>
    {
        public GetBookContentQuery(int libraryId, int bookId, string language, string mimeType, int? accountId)
            : base(libraryId)
        {
            BookId = bookId;
            MimeType = mimeType;
            AccountId = accountId;
            Language = language;
        }

        public int BookId { get; set; }

        public string MimeType { get; set; }
        public int? AccountId { get; }
        public string Language { get; set; }
    }

    public class GetBookContentQueryHandler : QueryHandlerAsync<GetBookContentQuery, BookContentModel>
    {
        private readonly ILibraryRepository _libraryRepository;
        private readonly IBookRepository _bookRepository;
        private readonly IFileRepository _fileRepository;

        public GetBookContentQueryHandler(ILibraryRepository libraryRepository, IBookRepository bookRepository, IFileRepository fileRepository)
        {
            _libraryRepository = libraryRepository;
            _bookRepository = bookRepository;
            _fileRepository = fileRepository;
        }

        public override async Task<BookContentModel> ExecuteAsync(GetBookContentQuery command, CancellationToken cancellationToken = new CancellationToken())
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

            var bookContent = await _bookRepository.GetBookContent(command.LibraryId, command.BookId, command.Language, command.MimeType, cancellationToken);
            if (bookContent != null)
            {
                if (command.AccountId.HasValue)
                {
                    await _bookRepository.AddRecentBook(command.LibraryId, command.AccountId.Value, command.BookId, cancellationToken);
                }

                if (book.IsPublic)
                {
                    bookContent.ContentUrl = await ImageHelper.TryConvertToPublicFile(bookContent.FileId, _fileRepository, cancellationToken);
                }
            }

            return bookContent;
        }
    }
}
