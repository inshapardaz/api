using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Entities.Library;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Library
{
    public class AddBookRequest : RequestBase
    {
        public AddBookRequest(Book book)
        {
            Book = book;
        }

        public Book Book { get; }

        public Book Result { get; set; }
    }

    public class AddBookRequestHandler : RequestHandlerAsync<AddBookRequest>
    {
        private readonly IBookRepository _bookRepository;

        public AddBookRequestHandler(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }

        public override async Task<AddBookRequest> HandleAsync(AddBookRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            command.Result= await _bookRepository.AddBook(command.Book, cancellationToken);

            return await base.HandleAsync(command, cancellationToken);
        }
    } 
}
