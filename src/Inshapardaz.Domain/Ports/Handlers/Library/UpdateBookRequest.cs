using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Library
{
    public class UpdateBookRequest : RequestBase
    {
        public UpdateBookRequest(BookModel book)
        {
            Book = book;
        }

        public BookModel Book { get; }

        public RequestResult Result { get; set; } = new RequestResult();

        public class RequestResult
        {
            public BookModel Book { get; set; }

            public bool HasAddedNew { get; set; }
        }
    }

    public class UpdateBookRequestHandler : RequestHandlerAsync<UpdateBookRequest>
    {
        private readonly IBookRepository _bookRepository;

        public UpdateBookRequestHandler(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }

        public override async Task<UpdateBookRequest> HandleAsync(UpdateBookRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            var result = await _bookRepository.GetBookById(command.Book.Id, cancellationToken);

            if (result == null)
            {
                var author = command.Book;
                author.Id = default(int);
                command.Result.Book =  await  _bookRepository.AddBook(author, cancellationToken);
                command.Result.HasAddedNew = true;
            }
            else
            {
                await _bookRepository.UpdateBook(command.Book, cancellationToken);
                command.Result.Book = command.Book;
            }

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}