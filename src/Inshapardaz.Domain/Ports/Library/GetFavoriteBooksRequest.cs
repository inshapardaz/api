using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Entities;
using Inshapardaz.Domain.Entities.Library;
using Inshapardaz.Domain.Exception;
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
        private readonly IFavoriteRepository _favoriteRepository;

        public GetFavoriteBooksRequestHandler(IFavoriteRepository favoriteRepository)
        {
            _favoriteRepository = favoriteRepository;
        }

        public override async Task<GetFavoriteBooksRequest> HandleAsync(GetFavoriteBooksRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            if (command.UserId == Guid.Empty)
            {
                throw new NotFoundException();
            }

            var books = await _favoriteRepository.GetFavoriteBooksByUser(command.UserId, command.PageNumber, command.PageSize, cancellationToken);
            foreach (var book in books.Data) book.IsFavorite = true;
            command.Result = books;
            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
