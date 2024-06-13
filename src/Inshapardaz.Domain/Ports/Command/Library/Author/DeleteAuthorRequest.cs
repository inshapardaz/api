using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Ports.Command.File;
using Paramore.Brighter;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Command.Library.Author;

public class DeleteAuthorRequest : LibraryBaseCommand
{
    public DeleteAuthorRequest(int libraryId, int authorId)
        : base(libraryId)
    {
        AuthorId = authorId;
    }

    public int AuthorId { get; }
}

public class DeleteAuthorRequestHandler : RequestHandlerAsync<DeleteAuthorRequest>
{
    private readonly IAuthorRepository _authorRepository;
    private readonly IAmACommandProcessor _commandProcessor;

    public DeleteAuthorRequestHandler(IAuthorRepository authorRepository,
        IAmACommandProcessor commandProcessor)
    {
        _authorRepository = authorRepository;
        _commandProcessor = commandProcessor;
    }

    [LibraryAuthorize(1, Role.LibraryAdmin)]
    public override async Task<DeleteAuthorRequest> HandleAsync(DeleteAuthorRequest command, CancellationToken cancellationToken = new CancellationToken())
    {
        var author = await _authorRepository.GetAuthorById(command.LibraryId, command.AuthorId, cancellationToken);
        if (author != null)
        {
            await _commandProcessor.SendAsync(new DeleteFileCommand(author.ImageId), cancellationToken: cancellationToken);
            await _authorRepository.DeleteAuthor(command.LibraryId, command.AuthorId, cancellationToken);
        }

        return await base.HandleAsync(command, cancellationToken);
    }
}
