using Inshapardaz.Domain.Adapters.Repositories;
using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Ports.Command.File;
using Paramore.Brighter;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Command.Library.Book;

public class DeleteBookContentRequest : BookRequest
{
    public DeleteBookContentRequest(int libraryId, int bookId, int contentId)
        : base(libraryId, bookId)
    {
        ContentId = contentId;
    }

    public int ContentId { get; }
}

public class DeleteBookContentRequestHandler : RequestHandlerAsync<DeleteBookContentRequest>
{
    private readonly IBookRepository _bookRepository;
    private readonly IAmACommandProcessor _commandProcessor;

    public DeleteBookContentRequestHandler(IBookRepository bookRepository, IAmACommandProcessor commandProcessor)
    {
        _bookRepository = bookRepository;
        _commandProcessor = commandProcessor;
    }

    [LibraryAuthorize(1, Role.LibraryAdmin, Role.Writer)]
    public override async Task<DeleteBookContentRequest> HandleAsync(DeleteBookContentRequest command, CancellationToken cancellationToken = new CancellationToken())
    {
        var content = await _bookRepository.GetBookContent(command.LibraryId, command.BookId, command.ContentId, cancellationToken);
        if (content != null)
        {
            await _commandProcessor.SendAsync(new DeleteFileCommand(command.ContentId), cancellationToken: cancellationToken);
            await _bookRepository.DeleteBookContent(command.LibraryId, command.BookId, command.ContentId, cancellationToken);
        }

        return await base.HandleAsync(command, cancellationToken);
    }
}
