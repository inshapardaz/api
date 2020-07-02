using Inshapardaz.Domain.Helpers;
using Inshapardaz.Domain.Models.Handlers.Library;
using Inshapardaz.Domain.Repositories;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Darker;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Models.Library
{
    public class GetBookByIdQuery : LibraryAuthorisedQuery<BookModel>
    {
        public GetBookByIdQuery(int libraryId, int bookId, Guid? userId)
            : base(libraryId, userId)
        {
            BookId = bookId;
        }

        public int BookId { get; private set; }
    }

    public class GetBookByIdQueryHandler : QueryHandlerAsync<GetBookByIdQuery, BookModel>
    {
        private readonly IBookRepository _bookRepository;
        private readonly IFileRepository _fileRepository;

        public GetBookByIdQueryHandler(IBookRepository bookRepository, IFileRepository fileRepository)
        {
            _bookRepository = bookRepository;
            _fileRepository = fileRepository;
        }

        public override async Task<BookModel> ExecuteAsync(GetBookByIdQuery command, CancellationToken cancellationToken = new CancellationToken())
        {
            var book = await _bookRepository.GetBookById(command.LibraryId, command.BookId, command.UserId, cancellationToken);
            if (book != null)
            {
                if (book.ImageId.HasValue)
                {
                    book.ImageUrl = await ImageHelper.TryConvertToPublicFile(book.ImageId.Value, _fileRepository, cancellationToken);
                }

                var contents = await _bookRepository.GetBookContents(command.LibraryId, command.BookId, cancellationToken);

                book.Contents = contents.ToList();
            }

            return book;
        }
    }
}
