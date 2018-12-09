using System;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Entities;
using Inshapardaz.Domain.Entities.Library;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Library
{
    public class GetBooksRequest : RequestBase
    {
        public GetBooksRequest(int pageNumber, int pageSize)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
        }

        public int PageNumber { get; private set; }

        public int PageSize { get; private set; }

        public Page<Book> Result { get; set; }

        public string Query { get; set; }
    }

    public class GetBooksRequestHandler : RequestHandlerAsync<GetBooksRequest>
    {
        private readonly IBookRepository _bookRepository;

        public GetBooksRequestHandler(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }

        public override async Task<GetBooksRequest> HandleAsync(GetBooksRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            if (string.IsNullOrWhiteSpace(command.Query))
            {
                var books = await _bookRepository.GetBooks(command.PageNumber, command.PageSize, cancellationToken);
                command.Result = books;
            }
            else
            {
                var books = await _bookRepository.SearchBooks(command.Query, command.PageNumber, command.PageSize, cancellationToken);
                command.Result = books;
            }

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
