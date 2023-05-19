using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Models.Handlers.Library;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Brighter;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Handlers.Library.BookShelf
{
    public class DeleteBookFromBookShelfRequest : LibraryBaseCommand
    {
        public DeleteBookFromBookShelfRequest(int libraryId, int bookShelfId, int bookId, int accountId)
            : base(libraryId)
        {
            BookShelfId = bookShelfId;
            BookId = bookId;
            AccountId = accountId;
        }

        public int BookShelfId { get; }
        public int BookId { get; }
        public int AccountId { get; }
    }

    public class DeleteBookFromBookShelfRequestHandler : RequestHandlerAsync<DeleteBookFromBookShelfRequest>
    {
        private readonly IBookShelfRepository _bookShelfRepository;
        private readonly IBookRepository _bookRepository;

        public DeleteBookFromBookShelfRequestHandler(IBookShelfRepository bookShelfRepository, IBookRepository bookRepository)
        {
            _bookShelfRepository = bookShelfRepository;
            _bookRepository = bookRepository;
        }

        public override async Task<DeleteBookFromBookShelfRequest> HandleAsync(DeleteBookFromBookShelfRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            var book = await _bookRepository.GetBookById(command.LibraryId, command.BookId, null, cancellationToken);
            var bookShelf = await _bookShelfRepository.GetBookShelfById(command.LibraryId, command.BookShelfId, cancellationToken);

            if (book == null || bookShelf == null)
            {
                return await base.HandleAsync(command, cancellationToken);
            }

            if (bookShelf.AccountId != command.AccountId)
            {
                throw new UnauthorizedException();
            }

            await _bookShelfRepository.RemoveBookFromBookShelf(command.LibraryId, command.BookShelfId, command.BookId, cancellationToken);
            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
