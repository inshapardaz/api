using Inshapardaz.Domain.Helpers;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Domain.Ports.Handlers.Library;
using Inshapardaz.Domain.Repositories;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Darker;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Library
{
    public class GetBooksQuery : LibraryAuthorisedQuery<Page<BookModel>>
    {
        public GetBooksQuery(int libraryId, int pageNumber, int pageSize, Guid? userId)
            : base(libraryId, userId)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
        }

        public int PageNumber { get; private set; }

        public int PageSize { get; private set; }

        public string Query { get; set; }
    }

    public class GetBooksQueryHandler : QueryHandlerAsync<GetBooksQuery, Page<BookModel>>
    {
        private readonly IBookRepository _bookRepository;
        private readonly IFileRepository _fileRepository;

        public GetBooksQueryHandler(IBookRepository bookRepository, IFileRepository fileRepository)
        {
            _bookRepository = bookRepository;
            _fileRepository = fileRepository;
        }

        public override async Task<Page<BookModel>> ExecuteAsync(GetBooksQuery command, CancellationToken cancellationToken = new CancellationToken())
        {
            var books = string.IsNullOrWhiteSpace(command.Query)
             ? await _bookRepository.GetBooks(command.LibraryId, command.PageNumber, command.PageSize, command.UserId, cancellationToken)
             : await _bookRepository.SearchBooks(command.LibraryId, command.Query, command.PageNumber, command.PageSize, command.UserId, cancellationToken);

            foreach (var book in books.Data)
            {
                if (book != null && book.ImageId.HasValue)
                {
                    book.ImageUrl = await ImageHelper.TryConvertToPublicFile(book.ImageId.Value, _fileRepository, cancellationToken);
                }

                var contents = await _bookRepository.GetBookContents(command.LibraryId, book.Id, cancellationToken);

                book.Contents = contents.ToList();
            }

            return books;
        }
    }
}
