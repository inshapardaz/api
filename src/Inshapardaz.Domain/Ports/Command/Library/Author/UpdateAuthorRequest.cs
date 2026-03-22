using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Command.Library.Author;

public class UpdateAuthorRequest(int libraryId, AuthorModel author) : LibraryBaseCommand(libraryId)
{
    public AuthorModel Author { get; } = author;

    public RequestResult Result { get; set; } = new RequestResult();

    public class RequestResult
    {
        public AuthorModel Author { get; set; }

        public bool HasAddedNew { get; set; }
    }
}

public class UpdateAuthorRequestHandler(IAuthorRepository authorRepository) : RequestHandlerAsync<UpdateAuthorRequest>
{
    [LibraryAuthorize(1, Role.LibraryAdmin, Role.Writer)]
    public override async Task<UpdateAuthorRequest> HandleAsync(UpdateAuthorRequest command, CancellationToken cancellationToken = new CancellationToken())
    {
        var result = await authorRepository.GetAuthorById(command.LibraryId, command.Author.Id, cancellationToken);

        if (result == null)
        {
            var author = command.Author;
            author.Id = default;
            command.Result.Author = await authorRepository.AddAuthor(command.LibraryId, author, cancellationToken);
            command.Result.HasAddedNew = true;
        }
        else
        {
            result.Name = command.Author.Name;
            result.Description = command.Author.Description;
            result.AuthorType = command.Author.AuthorType;
            await authorRepository.UpdateAuthor(command.LibraryId, result, cancellationToken);
            command.Result.Author = command.Author;
        }

        return await base.HandleAsync(command, cancellationToken);
    }
}
