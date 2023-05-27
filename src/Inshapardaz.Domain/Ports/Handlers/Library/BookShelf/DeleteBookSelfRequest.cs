using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Models.Handlers.Library;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Brighter;
using System.Threading;
using System.Threading.Tasks;
using static Lucene.Net.Util.Fst.Util;

namespace Inshapardaz.Domain.Ports.Handlers.Library.BookShelf
{
    public class DeleteBookSelfRequest : LibraryBaseCommand
    {
        public DeleteBookSelfRequest(int libraryId, int bookShelfId, int accountId)
            : base(libraryId)
        {
            BookShelfId = bookShelfId;
            AccountId = accountId;
        }

        public int BookShelfId { get; }
        public int AccountId { get; }
    }

    public class DeleteBookSelfRequestHandler : RequestHandlerAsync<DeleteBookSelfRequest>
    {
        private readonly IBookShelfRepository _bookShelfRepository;

        public DeleteBookSelfRequestHandler(IBookShelfRepository bookShelfRepository)
        {
            _bookShelfRepository = bookShelfRepository;
        }

        public override async Task<DeleteBookSelfRequest> HandleAsync(DeleteBookSelfRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            var result = await _bookShelfRepository.GetBookShelfById(command.LibraryId, command.BookShelfId, cancellationToken);

            if (result != null && result.AccountId != command.AccountId)
            {
                throw new ForbiddenException();
            }

            await _bookShelfRepository.DeleteBookShelf(command.LibraryId, command.BookShelfId, cancellationToken);

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
