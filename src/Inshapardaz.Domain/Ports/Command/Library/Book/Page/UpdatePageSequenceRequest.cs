using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Command.Library.Book.Page;

public class UpdateBookPageSequenceRequest(int libraryId, int bookId, int oldSequenceNumber, int newSequenceNumber)
    : BookRequest(libraryId, bookId)
{
    public IEnumerable<BookPageModel> BookPages { get; }
    public int OldSequenceNumber { get; } = oldSequenceNumber;
    public int NewSequenceNumber { get; } = newSequenceNumber;
}

public class UpdatePageSequenceRequestHandler(IBookPageRepository bookPageRepository)
    : RequestHandlerAsync<UpdateBookPageSequenceRequest>
{
    [LibraryAuthorize(1, Role.LibraryAdmin, Role.Writer)]
    public override async Task<UpdateBookPageSequenceRequest> HandleAsync(UpdateBookPageSequenceRequest command, CancellationToken cancellationToken = new CancellationToken())
    {
        // No Change in page sequence
        if (command.OldSequenceNumber == command.NewSequenceNumber)
        {
            return await base.HandleAsync(command, cancellationToken);
        }

        var page = await bookPageRepository.GetPageBySequenceNumber(command.LibraryId, command.BookId, command.OldSequenceNumber, cancellationToken);

        // Check if the page exist
        if (page == null)
        {
            throw new NotFoundException();
        }

        await bookPageRepository.UpdatePageSequenceNumber(command.LibraryId, command.BookId, command.OldSequenceNumber, command.NewSequenceNumber, cancellationToken);

        return await base.HandleAsync(command, cancellationToken);
    }
}
