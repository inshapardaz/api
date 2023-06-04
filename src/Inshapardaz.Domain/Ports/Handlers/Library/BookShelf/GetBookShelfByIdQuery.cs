using Inshapardaz.Domain.Exception;
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
        public GetBookShelfByIdQuery(int libraryId, int bookShelfId, int? accountId)
            : base(libraryId)
        {
            BookShelfId = bookShelfId;
            AccountId = accountId;
        }

        public int BookShelfId { get; }
        public int? AccountId { get; }
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
            var bookShelf = await _bookShelfRepository.GetBookShelfById(command.LibraryId, command.BookShelfId, cancellationToken);

            if (bookShelf != null && !bookShelf.IsPublic && bookShelf.AccountId != command.AccountId)
            {
                throw new NotFoundException();
            }
            if (bookShelf != null && bookShelf.ImageId.HasValue)
            {
                bookShelf.ImageUrl = await ImageHelper.TryConvertToPublicFile(bookShelf.ImageId.Value, _fileRepository, cancellationToken);
            }

            return bookShelf;
        }
    }
}
