using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using Paramore.Brighter;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Command.Library.Author;

public class AddAuthorRequest : LibraryBaseCommand
{
    public AddAuthorRequest(int libraryId, AuthorModel author)
        : base(libraryId)
    {
        Author = author;
    }

    public AuthorModel Author { get; }

    public AuthorModel Result { get; set; }
}

public class AddAuthorRequestHandler : RequestHandlerAsync<AddAuthorRequest>
{
    private readonly IAuthorRepository _authorRepository;

    public AddAuthorRequestHandler(IAuthorRepository authorRepository)
    {
        _authorRepository = authorRepository;
    }

    [LibraryAuthorize(1, Role.LibraryAdmin, Role.Writer)]
    public override async Task<AddAuthorRequest> HandleAsync(AddAuthorRequest command, CancellationToken cancellationToken = new CancellationToken())
    {
        command.Result = await _authorRepository.AddAuthor(command.LibraryId, command.Author, cancellationToken);

        return await base.HandleAsync(command, cancellationToken);
    }
}
