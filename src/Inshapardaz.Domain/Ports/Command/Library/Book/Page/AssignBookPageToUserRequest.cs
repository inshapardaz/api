using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Domain.Ports.Query.Library.Book.Page;
using Paramore.Brighter;
using Paramore.Darker;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Command.Library.Book.Page;

public class AssignBookPageRequest : LibraryBaseCommand
{
    public AssignBookPageRequest(int libraryId, int bookId, int sequenceNumber, int? accountId)
    : base(libraryId)
    {
        BookId = bookId;
        SequenceNumber = sequenceNumber;
        AccountId = accountId;
    }

    public BookPageModel Result { get; set; }
    public int BookId { get; set; }
    public int SequenceNumber { get; set; }
    public int? AccountId { get; private set; }
}

public class AssignBookPageRequestHandler : RequestHandlerAsync<AssignBookPageRequest>
{
    private readonly IBookRepository _bookRepository;
    private readonly IBookPageRepository _bookPageRepository;
    private readonly IQueryProcessor _queryProcessor;

    public AssignBookPageRequestHandler(IBookRepository bookRepository,
                                     IBookPageRepository bookPageRepository,
                                     IQueryProcessor queryProcessor)
    {
        _bookRepository = bookRepository;
        _bookPageRepository = bookPageRepository;
        _queryProcessor = queryProcessor;
    }

    [LibraryAuthorize(1, Role.LibraryAdmin, Role.Writer)]
    public override async Task<AssignBookPageRequest> HandleAsync(AssignBookPageRequest command, CancellationToken cancellationToken = new CancellationToken())
    {
        var page = await _bookPageRepository.GetPageBySequenceNumber(command.LibraryId, command.BookId, command.SequenceNumber, cancellationToken);
        if (page == null)
        {
            throw new BadRequestException();
        }

        if (page.Status == EditingStatus.Available || page.Status == EditingStatus.Typing)
        {
            await _bookPageRepository.UpdateWriterAssignment(command.LibraryId, command.BookId, command.SequenceNumber, command.AccountId, cancellationToken);
        }
        else if (page.Status == EditingStatus.Typed || page.Status == EditingStatus.InReview)
        {
            await _bookPageRepository.UpdateReviewerAssignment(command.LibraryId, command.BookId, command.SequenceNumber, command.AccountId, cancellationToken);
        }
        else
        {
            throw new BadRequestException("Page status does not allow it to be assigned");
        }

        command.Result = await _queryProcessor.ExecuteAsync(new GetBookPageByNumberQuery(command.LibraryId, command.BookId, command.SequenceNumber), cancellationToken);


        return await base.HandleAsync(command, cancellationToken);
    }
}
