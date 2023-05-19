using Inshapardaz.Domain.Helpers;
using Inshapardaz.Domain.Models.Handlers.Library;
using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Domain.Repositories;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Darker;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Handlers.Library.BookShelf
{
    public class GetBookShelfByIdQuery : LibraryBaseQuery<BookShelfModel>
    {
        public GetBookShelfByIdQuery(int libraryId, int bookShelfId)
            : base(libraryId)
        {
            BookShelfId = bookShelfId;
        }

        public int BookShelfId { get; }
    }

    public class GetBookShelfByIdQueryHandler : QueryHandlerAsync<GetBookShelfByIdQuery, BookShelfModel>
    {
        private readonly IBookShelfRepository _bookShelfRepository;
        private readonly IFileRepository _fileRepository;

        public GetBookShelfByIdQueryHandler(IBookShelfRepository BookShelfRepository, IFileRepository fileRepository)
        {
            _bookShelfRepository = BookShelfRepository;
            _fileRepository = fileRepository;
        }

        public override async Task<BookShelfModel> ExecuteAsync(GetBookShelfByIdQuery command, CancellationToken cancellationToken = new CancellationToken())
        {
            var BookShelf = await _bookShelfRepository.GetBookShelfById(command.LibraryId, command.BookShelfId, cancellationToken);

            if (BookShelf != null && BookShelf.ImageId.HasValue)
            {
                BookShelf.ImageUrl = await ImageHelper.TryConvertToPublicFile(BookShelf.ImageId.Value, _fileRepository, cancellationToken);
            }

            return BookShelf;
        }
    }
}
