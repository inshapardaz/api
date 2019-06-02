using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Entities;
using Inshapardaz.Domain.Entities.Library;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Library
{
    public class GetLatestBooksRequest : RequestBase
    {
        public GetLatestBooksRequest()
        {
        }

        public IEnumerable<Book> Result { get; set; }
    }

    public class GetLatestBooksRequestHandler : RequestHandlerAsync<GetLatestBooksRequest>
    {
        private readonly IBookRepository _bookRepository;

        public GetLatestBooksRequestHandler(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }

        public override async Task<GetLatestBooksRequest> HandleAsync(GetLatestBooksRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            var books = await _bookRepository.GetLatestBooks(cancellationToken);
            command.Result = books;
            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
