using System;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Entities;
using Inshapardaz.Domain.Entities.Library;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Darker;

namespace Inshapardaz.Domain.Ports.Library
{
    public class GetFavoriteBooksQuery : IQuery<Page<Book>>
    {
        public GetFavoriteBooksQuery(Guid userId, int pageNumber, int pageSize)
        {
            UserId = userId;
            PageNumber = pageNumber;
            PageSize = pageSize;
        }

        public Guid UserId { get; private set;}

        public int PageNumber { get; private set; }

        public int PageSize { get; private set; }
    }

    public class GetFavoriteBooksQueryHandler : QueryHandlerAsync<GetFavoriteBooksQuery, Page<Book>>
    {
        private readonly IFavoriteRepository _favoriteRepository;

        public GetFavoriteBooksQueryHandler(IFavoriteRepository favoriteRepository)
        {
            _favoriteRepository = favoriteRepository;
        }

        public override async Task<Page<Book>> ExecuteAsync(GetFavoriteBooksQuery command, CancellationToken cancellationToken = new CancellationToken())
        {
            if (command.UserId == Guid.Empty)
            {
                throw new NotFoundException();
            }

            var books = await _favoriteRepository.GetFavoriteBooksByUser(command.UserId, command.PageNumber, command.PageSize, cancellationToken);
            foreach (var book in books.Data) book.IsFavorite = true;
            return books;
        }
    }
}
