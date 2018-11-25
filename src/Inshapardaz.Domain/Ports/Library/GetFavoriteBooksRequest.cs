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
    public class GetFavoriteBooksRequest : RequestBase
    {
        public GetFavoriteBooksRequest(Guid userId)
        {
        }

        public Guid UserId { get; private set;}
        
        public IEnumerable<Book> Result { get; set; }
    }

    public class GetFavoriteBooksRequestHandler : RequestHandlerAsync<GetFavoriteBooksRequest>
    {
        private readonly IBookRepository _bookRepository;

        public GetFavoriteBooksRequestHandler(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }

        public override async Task<GetFavoriteBooksRequest> HandleAsync(GetFavoriteBooksRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
            var books = await _bookRepository.GtLatestBooks(cancellationToken);
            command.Result = books;
            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
