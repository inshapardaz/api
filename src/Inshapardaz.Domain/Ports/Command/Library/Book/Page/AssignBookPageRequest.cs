using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Domain.Ports.Query.Library.Book.Page;
using Paramore.Brighter;
using Paramore.Darker;

namespace Inshapardaz.Domain.Ports.Command.Library.Book.Page;

public class AssignBookPageToUserRequest(int libraryId, int bookId, int sequenceNumber, int? accountId)
    : LibraryBaseCommand(libraryId)
{
    public BookPageModel Result { get; set; }
    public int BookId { get; set; } = bookId;
    public int SequenceNumber { get; set; } = sequenceNumber;
    public int? AccountId { get; private set; } = accountId;
}

public class AssignBookPageToUserRequestHandler(IBookPageRepository bookPageRepository, IQueryProcessor queryProcessor)
    : RequestHandlerAsync<AssignBookPageToUserRequest>
{
    [LibraryAuthorize(1, Role.LibraryAdmin, Role.Writer)]
    public override async Task<AssignBookPageToUserRequest> HandleAsync(AssignBookPageToUserRequest command, CancellationToken cancellationToken = new CancellationToken())
    {
        var page = await bookPageRepository.GetPageBySequenceNumber(command.LibraryId, command.BookId, command.SequenceNumber, cancellationToken);
        if (page == null)
        {
            throw new BadRequestException();
        }

        if (page.Status == EditingStatus.Available || page.Status == EditingStatus.Typing)
        {
            await bookPageRepository.UpdateWriterAssignment(command.LibraryId, command.BookId, command.SequenceNumber, command.AccountId, cancellationToken);
        }
        else if (page.Status == EditingStatus.Typed || page.Status == EditingStatus.InReview)
        {
            await bookPageRepository.UpdateReviewerAssignment(command.LibraryId, command.BookId, command.SequenceNumber, command.AccountId, cancellationToken);
        }
        else
        {
            throw new BadRequestException("Page status does not allow it to be assigned");
        }

        command.Result = await queryProcessor.ExecuteAsync(new GetBookPageByNumberQuery(command.LibraryId, command.BookId, command.SequenceNumber), cancellationToken);
        return await base.HandleAsync(command, cancellationToken);
    }
}
