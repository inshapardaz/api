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
        public GetFavoriteBooksRequest(Guid userId, int pageNumber, int pageSize)
        {
            UserId = userId;
            PageNumber = pageNumber;
            PageSize = pageSize;
        }

        public Guid UserId { get; private set;}

        public int PageNumber { get; private set; }

        public int PageSize { get; private set; }

        public Page<Book> Result { get; set; }
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
            var books = await _bookRepository.GetFavoriteBooksByUser(command.UserId, command.PageNumber, command.PageSize, cancellationToken);
            command.Result = books;
            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
