using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Domain.Ports.Command;
using Paramore.Brighter;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Handlers.Library.Book.Page
{
    public class UpdateBookPageSequenceRequest : BookRequest
    {
        public UpdateBookPageSequenceRequest(int libraryId, int bookId, int oldSequenceNumber, int newSequenceNumber)
            : base(libraryId, bookId)
        {
            OldSequenceNumber = oldSequenceNumber;
            NewSequenceNumber = newSequenceNumber;
        }

        public IEnumerable<BookPageModel> BookPages { get; }
        public int OldSequenceNumber { get; }
        public int NewSequenceNumber { get; }
    }

    public class UpdatePageSequenceRequestHandler : RequestHandlerAsync<UpdateBookPageSequenceRequest>
    {
        private readonly IBookPageRepository _bookPageRepository;

        public UpdatePageSequenceRequestHandler(IBookPageRepository bookPageRepository)
        {
            _bookPageRepository = bookPageRepository;
        }

        [LibraryAuthorize(1, Role.LibraryAdmin, Role.Writer)]
        public override async Task<UpdateBookPageSequenceRequest> HandleAsync(UpdateBookPageSequenceRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            // No Change in page sequence
            if (command.OldSequenceNumber == command.NewSequenceNumber)
            {
                return await base.HandleAsync(command, cancellationToken);
            }

            var page = await _bookPageRepository.GetPageBySequenceNumber(command.LibraryId, command.BookId, command.OldSequenceNumber, cancellationToken);

            // Check if the page exist
            if (page == null)
            {
                throw new NotFoundException();
            }

            await _bookPageRepository.UpdatePageSequenceNumber(command.LibraryId, command.BookId, command.OldSequenceNumber, command.NewSequenceNumber, cancellationToken);

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
