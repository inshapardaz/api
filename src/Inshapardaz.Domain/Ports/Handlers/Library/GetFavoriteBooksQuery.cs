using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Domain.Ports.Handlers.Library;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Darker;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Library
{
    public class GetFavoriteBooksQuery : LibraryAuthorisedQuery<Page<BookModel>>
    {
        public GetFavoriteBooksQuery(int libraryId, Guid userId, int pageNumber, int pageSize)
            : base(libraryId, userId)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
        }

        public int PageNumber { get; private set; }

        public int PageSize { get; private set; }
    }

    public class GetFavoriteBooksQueryHandler : QueryHandlerAsync<GetFavoriteBooksQuery, Page<BookModel>>
    {
        private readonly IBookRepository _bookRepository;

        public GetFavoriteBooksQueryHandler(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }

        public override async Task<Page<BookModel>> ExecuteAsync(GetFavoriteBooksQuery command, CancellationToken cancellationToken = new CancellationToken())
        {
            if (command.UserId == Guid.Empty)
            {
                throw new NotFoundException();
            }

            return await _bookRepository.GetFavoriteBooksByUser(command.LibraryId, command.UserId, command.PageNumber, command.PageSize, cancellationToken);
        }
    }
}
