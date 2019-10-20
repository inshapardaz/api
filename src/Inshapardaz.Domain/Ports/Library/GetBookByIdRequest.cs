using System;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Entities.Library;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Library
{
    public class GetBookByIdRequest : BookRequest
    {
        public GetBookByIdRequest(int bookId, Guid userId)
            : base(bookId, userId)
        {
        }

        public Book Result { get; set; }
    }

    public class GetBookByIdRequestHandler : RequestHandlerAsync<GetBookByIdRequest>
    {
        private readonly IBookRepository _bookRepository;
        private readonly IFavoriteRepository _favoriteRepository;

        public GetBookByIdRequestHandler(IBookRepository bookRepository, IFavoriteRepository favoriteRepository)
        {
            _bookRepository = bookRepository;
            _favoriteRepository = favoriteRepository;
        }

        public override async Task<GetBookByIdRequest> HandleAsync(GetBookByIdRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            var book = await _bookRepository.GetBookById(command.BookId, cancellationToken);
            if (book!= null && command.UserId != Guid.Empty)
            {
                book.IsFavorite = await _favoriteRepository.IsBookFavorite(command.UserId, command.BookId, cancellationToken);
            }

            command.Result = book;
            
            return await base.HandleAsync(command, cancellationToken);
        }
    }
}

