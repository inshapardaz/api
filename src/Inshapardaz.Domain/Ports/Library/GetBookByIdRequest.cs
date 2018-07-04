using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Entities.Library;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Library
{
    public class GetBookByIdRequest : RequestBase
    {
        public GetBookByIdRequest(int bookId)
        {
            BookId = bookId;
        }

        public Book Result { get; set; }
        public int BookId { get; }
    }

    public class GetBookByIdRequestHandler : RequestHandlerAsync<GetBookByIdRequest>
    {
        private readonly IBookRepository _bookRepository;

        public GetBookByIdRequestHandler(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }

        public override async Task<GetBookByIdRequest> HandleAsync(GetBookByIdRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            var book = await _bookRepository.GetBookById(command.BookId, cancellationToken);
            command.Result = book;
            
            return await base.HandleAsync(command, cancellationToken);
        }
    }
}

