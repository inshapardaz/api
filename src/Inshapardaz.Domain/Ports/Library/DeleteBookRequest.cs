using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Library
{
    public class DeleteBookRequest : RequestBase
    {
        public DeleteBookRequest(int bookId)
        {
            BookId = bookId;
        }

        public int BookId { get; }
    }

    public class DeleteBookRequestHandler : RequestHandlerAsync<DeleteBookRequest>
    {
        private readonly IBookRepository _bookRepository;

        public DeleteBookRequestHandler(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }

        public override async Task<DeleteBookRequest> HandleAsync(DeleteBookRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            await _bookRepository.DeleteBook(command.BookId, cancellationToken);

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}