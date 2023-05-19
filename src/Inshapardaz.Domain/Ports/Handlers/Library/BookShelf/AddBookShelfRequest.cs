using Inshapardaz.Domain.Models.Handlers.Library;
using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Brighter;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Handlers.Library.BookShelf
{
    public class AddBookShelfRequest : LibraryBaseCommand
    {
        public AddBookShelfRequest(int libraryId, BookShelfModel bookShelf)
            : base(libraryId)
        {
            BookShelf = bookShelf;
        }

        public BookShelfModel BookShelf { get; }
        public BookShelfModel Result { get; set; }
    }

    public class AddBookShelfRequestHandler : RequestHandlerAsync<AddBookShelfRequest>
    {
        private readonly IBookShelfRepository _bookShelfRepository;

        public AddBookShelfRequestHandler(IBookShelfRepository bookShelfRepository)
        {
            _bookShelfRepository = bookShelfRepository;
        }

        public override async Task<AddBookShelfRequest> HandleAsync(AddBookShelfRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            command.Result = await _bookShelfRepository.AddBookShelf(command.LibraryId, command.BookShelf, cancellationToken);
            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
