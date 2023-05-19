using Inshapardaz.Domain.Helpers;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Handlers.Library;
using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Domain.Repositories;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Darker;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Handlers.Library.BookShelf
{
    public class GetBookShelfQuery : LibraryBaseQuery<Page<BookShelfModel>>
    {
        public GetBookShelfQuery(int libraryId, int pageNumber, int pageSize)
            : base(libraryId)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
        }

        public int PageNumber { get; private set; }

        public int PageSize { get; private set; }

        public string Query { get; set; }
    }

    public class GetBookShelfQueryHandler : QueryHandlerAsync<GetBookShelfQuery, Page<BookShelfModel>>
    {
        private readonly IBookShelfRepository _bookShelfRepository;
        private readonly IFileRepository _fileRepository;

        public GetBookShelfQueryHandler(IBookShelfRepository bookShelfRepository, IFileRepository fileRepository)
        {
            _bookShelfRepository = bookShelfRepository;
            _fileRepository = fileRepository;
        }

        public override async Task<Page<BookShelfModel>> ExecuteAsync(GetBookShelfQuery query, CancellationToken cancellationToken = new CancellationToken())
        {
            var BookShelf = string.IsNullOrWhiteSpace(query.Query)
             ? await _bookShelfRepository.GetBookShelves(query.LibraryId, query.PageNumber, query.PageSize, cancellationToken)
             : await _bookShelfRepository.FindBookShelves(query.LibraryId, query.Query, query.PageNumber, query.PageSize, cancellationToken);

            foreach (var author in BookShelf.Data)
            {
                if (author != null && author.ImageId.HasValue)
                {
                    author.ImageUrl = await ImageHelper.TryConvertToPublicFile(author.ImageId.Value, _fileRepository, cancellationToken);
                }
            }

            return BookShelf;
        }
    }
}
