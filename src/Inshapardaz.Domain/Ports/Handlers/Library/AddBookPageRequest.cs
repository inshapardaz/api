using Inshapardaz.Domain.Adapters;
using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Models.Handlers.Library;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Brighter;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Models.Library
{
    public class AddBookPageRequest : LibraryAuthorisedCommand
    {
        public AddBookPageRequest(ClaimsPrincipal claims, int libraryId, BookPageModel book)
        : base(claims, libraryId)
        {
            BookPage = book;
        }

        public BookPageModel BookPage { get; }

        public BookPageModel Result { get; set; }

        public bool IsAdded { get; set; }
    }

    public class AddBookPageRequestHandler : RequestHandlerAsync<AddBookPageRequest>
    {
        private readonly IBookRepository _bookRepository;
        private readonly IBookPageRepository _bookPageRepository;

        public AddBookPageRequestHandler(IBookRepository bookRepository,
                                         IBookPageRepository bookPageRepository)
        {
            _bookRepository = bookRepository;
            _bookPageRepository = bookPageRepository;
        }

        [Authorise(step: 1, HandlerTiming.Before, Permission.Admin, Permission.LibraryAdmin, Permission.Writer)]
        public override async Task<AddBookPageRequest> HandleAsync(AddBookPageRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            var book = await _bookRepository.GetBookById(command.LibraryId, command.BookPage.BookId, command.UserId, cancellationToken);
            if (book == null)
            {
                throw new BadRequestException();
            }

            var existingBookPage = await _bookPageRepository.GetPageByPageNumber(command.LibraryId, command.BookPage.BookId, command.BookPage.PageNumber, cancellationToken);

            if (existingBookPage == null)
            {
                command.Result = await _bookPageRepository.AddPage(command.LibraryId, command.BookPage.BookId, command.BookPage.PageNumber, command.BookPage.Text, 0, cancellationToken);
                command.IsAdded = true;
            }
            else
            {
                command.Result = await _bookPageRepository.UpdatePage(command.LibraryId, command.BookPage.BookId, command.BookPage.PageNumber, command.BookPage.Text, 0, cancellationToken);
            }

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
