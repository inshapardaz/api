using Inshapardaz.Domain.Models.Handlers.Library;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Brighter;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Handlers.Library.BookShelf
{
    public class DeleteBookSelfRequest : LibraryBaseCommand
    {
        public DeleteBookSelfRequest(int libraryId, int bookShelfId)
            : base(libraryId)
        {
            BookShelfId = bookShelfId;
        }

        public int BookShelfId { get; }
    }

    public class DeleteBookSelfRequestHandler : RequestHandlerAsync<DeleteBookSelfRequest>
    {
        private readonly IBookShelfRepository _bookShelfRepository;

        public DeleteBookSelfRequestHandler(IBookShelfRepository bookShelfRepository)
        {
            _bookShelfRepository = bookShelfRepository;
        }

        public override async Task<DeleteBookSelfRequest> HandleAsync(DeleteBookSelfRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            await _bookShelfRepository.DeleteBookShelf(command.LibraryId, command.BookShelfId, cancellationToken);

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
