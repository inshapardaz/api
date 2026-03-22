using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Command.Library.Author;

public class AddAuthorRequest(int libraryId, AuthorModel author) : LibraryBaseCommand(libraryId)
{
    public AuthorModel Author { get; } = author;

    public AuthorModel Result { get; set; }
}

public class AddAuthorRequestHandler(IAuthorRepository authorRepository) : RequestHandlerAsync<AddAuthorRequest>
{
    [LibraryAuthorize(1, Role.LibraryAdmin, Role.Writer)]
    public override async Task<AddAuthorRequest> HandleAsync(AddAuthorRequest command, CancellationToken cancellationToken = new CancellationToken())
    {
        command.Result = await authorRepository.AddAuthor(command.LibraryId, command.Author, cancellationToken);

        return await base.HandleAsync(command, cancellationToken);
    }
}
