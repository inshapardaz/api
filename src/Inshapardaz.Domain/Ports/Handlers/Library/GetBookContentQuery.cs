using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Helpers;
using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Domain.Ports.Handlers.Library;
using Inshapardaz.Domain.Repositories;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Darker;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Library
{
    public class GetBookContentQuery : LibraryAuthorisedQuery<BookContentModel>
    {
        public GetBookContentQuery(int libraryId, int bookId, string language, string mimeType, Guid userId)
            : base(libraryId, userId)
        {
            BookId = bookId;
            MimeType = mimeType;
            Language = language;
        }

        public int BookId { get; set; }

        public string MimeType { get; set; }

        public string Language { get; set; }
    }

    public class GetBookContentQueryHandler : QueryHandlerAsync<GetBookContentQuery, BookContentModel>
    {
        private readonly IBookRepository _bookRepository;
        private readonly IFileRepository _fileRepository;

        public GetBookContentQueryHandler(IBookRepository bookRepository, IFileRepository fileRepository)
        {
            _bookRepository = bookRepository;
            _fileRepository = fileRepository;
        }

        public override async Task<BookContentModel> ExecuteAsync(GetBookContentQuery command, CancellationToken cancellationToken = new CancellationToken())
        {
            var book = await _bookRepository.GetBookById(command.LibraryId, command.BookId, command.UserId, cancellationToken);
            if (book == null)
            {
                throw new NotFoundException();
            }

            if (!book.IsPublic && command.UserId == Guid.Empty)
            {
                throw new UnauthorizedException();
            }

            var bookContent = await _bookRepository.GetBookContent(command.LibraryId, command.BookId, command.Language, command.MimeType, cancellationToken);
            if (bookContent != null)
            {
                if (command.UserId != Guid.Empty)
                {
                    await _bookRepository.AddRecentBook(command.LibraryId, command.UserId, command.BookId, cancellationToken);
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
