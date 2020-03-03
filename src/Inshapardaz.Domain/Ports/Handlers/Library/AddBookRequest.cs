using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Domain.Ports.Handlers.Library;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Brighter;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Library
{
    public class AddBookRequest : LibraryAuthorisedCommand
    {
        public AddBookRequest(ClaimsPrincipal claims, int libraryId, BookModel book)
        : base(claims, libraryId)
        {
            Book = book;
        }

        public BookModel Book { get; }

        public BookModel Result { get; set; }
    }

    public class AddBookRequestHandler : RequestHandlerAsync<AddBookRequest>
    {
        private readonly IBookRepository _bookRepository;

        public AddBookRequestHandler(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }

        [Authorise(step: 1, HandlerTiming.Before)]
        public override async Task<AddBookRequest> HandleAsync(AddBookRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            command.Result = await _bookRepository.AddBook(command.LibraryId, command.Book, command.UserId, cancellationToken);

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
