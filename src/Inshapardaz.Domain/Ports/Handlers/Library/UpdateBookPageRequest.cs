using Inshapardaz.Domain.Adapters;
using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Models.Handlers.Library;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Brighter;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Models.Library
{
    public class UpdateBookPageRequest : LibraryBaseCommand
    {
        public UpdateBookPageRequest(int libraryId, int bookId, int sequenceNumber, int? accountId, BookPageModel book)
        : base(libraryId)
        {
            BookPage = book;
            BookPage.BookId = bookId;
            SequenceNumber = sequenceNumber;
            AccountId = accountId;
        }

        public BookPageModel BookPage { get; }

        public RequestResult Result { get; set; } = new RequestResult();
        public int SequenceNumber { get; set; }
        public int? AccountId { get; }

        public class RequestResult
        {
            public BookPageModel BookPage { get; set; }

            public bool HasAddedNew { get; set; }
        }
    }

    public class UpdateBookPageRequestHandler : RequestHandlerAsync<UpdateBookPageRequest>
    {
        private readonly IBookRepository _bookRepository;
        private readonly IBookPageRepository _bookPageRepository;

        public UpdateBookPageRequestHandler(IBookRepository bookRepository,
                                         IBookPageRepository bookPageRepository)
        {
            _bookRepository = bookRepository;
            _bookPageRepository = bookPageRepository;
        }

        public override async Task<UpdateBookPageRequest> HandleAsync(UpdateBookPageRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            var book = await _bookRepository.GetBookById(command.LibraryId, command.BookPage.BookId, command.AccountId, cancellationToken);
            if (book == null)
            {
                throw new BadRequestException();
            }

            var existingBookPage = await _bookPageRepository.GetPageBySequenceNumber(command.LibraryId, command.BookPage.BookId, command.BookPage.SequenceNumber, cancellationToken);

            if (existingBookPage == null)
            {
                command.Result.BookPage = await _bookPageRepository.AddPage(command.LibraryId, command.BookPage.BookId, command.BookPage.SequenceNumber, command.BookPage.Text, 0, cancellationToken);
                command.Result.HasAddedNew = true;
            }
            else
            {
                command.Result.BookPage = await _bookPageRepository.UpdatePage(command.LibraryId, command.BookPage.BookId, command.BookPage.SequenceNumber, command.BookPage.Text, 0, cancellationToken);
            }

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
