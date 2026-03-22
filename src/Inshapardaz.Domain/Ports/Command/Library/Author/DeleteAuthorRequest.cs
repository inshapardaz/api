using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Ports.Command.File;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Command.Library.Author;

public class DeleteAuthorRequest(int libraryId, int authorId) : LibraryBaseCommand(libraryId)
{
    public int AuthorId { get; } = authorId;
}

public class DeleteAuthorRequestHandler(
    IAuthorRepository authorRepository,
    IAmACommandProcessor commandProcessor)
    : RequestHandlerAsync<DeleteAuthorRequest>
{
    [LibraryAuthorize(1, Role.LibraryAdmin)]
    public override async Task<DeleteAuthorRequest> HandleAsync(DeleteAuthorRequest command, CancellationToken cancellationToken = new CancellationToken())
    {
        var author = await authorRepository.GetAuthorById(command.LibraryId, command.AuthorId, cancellationToken);
        if (author != null)
        {
            await commandProcessor.SendAsync(new DeleteFileCommand(author.ImageId), cancellationToken: cancellationToken);
            await authorRepository.DeleteAuthor(command.LibraryId, command.AuthorId, cancellationToken);
        }

        return await base.HandleAsync(command, cancellationToken);
    }
}
