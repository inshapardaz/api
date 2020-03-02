using System;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Domain.Ports.Handlers.Library;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Darker;

namespace Inshapardaz.Domain.Ports.Library
{
    public class GetBooksByCategoryQuery : LibraryAuthorisedQuery<Page<BookModel>>
    {
        public GetBooksByCategoryQuery(int libraryId, int categoryId, int pageNumber, int pageSize, Guid userId)
            : base(libraryId, userId)
        {
            CategoryId = categoryId;
            PageNumber = pageNumber;
            PageSize = pageSize;
        }

        public int CategoryId { get; private set; }

        public int PageNumber { get; private set; }

        public int PageSize { get; private set; }
    }

    public class GetBooksByCategoryQueryHandler : QueryHandlerAsync<GetBooksByCategoryQuery, Page<BookModel>>
    {
        private readonly IBookRepository _bookRepository;

        public GetBooksByCategoryQueryHandler(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }

        public override async Task<Page<BookModel>> ExecuteAsync(GetBooksByCategoryQuery command, CancellationToken cancellationToken = new CancellationToken())
        {
            return await _bookRepository.GetBooksByCategory(command.LibraryId, command.CategoryId, command.PageNumber, command.PageSize, command.UserId, cancellationToken);
        }
    }
}
