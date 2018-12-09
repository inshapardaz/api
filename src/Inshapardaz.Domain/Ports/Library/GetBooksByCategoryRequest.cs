using System;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Entities;
using Inshapardaz.Domain.Entities.Library;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Library
{
    public class GetBooksByCategoryRequest : RequestBase
    {
        public GetBooksByCategoryRequest(int categoryId, int pageNumber, int pageSize)
        {
            CategoryId = categoryId;
            PageNumber = pageNumber;
            PageSize = pageSize;
        }

        public int CategoryId { get; private set; }

        public int PageNumber { get; private set; }

        public int PageSize { get; private set; }

        public Guid UserId { get; set; }

        public Page<Book> Result { get; set; }
    }

    public class GetBooksByCategoryRequestHandler : RequestHandlerAsync<GetBooksByCategoryRequest>
    {
        private readonly IBookRepository _bookRepository;

        public GetBooksByCategoryRequestHandler(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }

        public override async Task<GetBooksByCategoryRequest> HandleAsync(GetBooksByCategoryRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            var books = await _bookRepository.GetBooksByCategory(command.CategoryId, command.PageNumber, command.PageSize, cancellationToken);
            command.Result = books;
            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
