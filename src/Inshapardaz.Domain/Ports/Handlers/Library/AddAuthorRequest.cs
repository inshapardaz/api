using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Domain.Models.Handlers.Library;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Brighter;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Models.Library
{
    public class AddAuthorRequest : LibraryAuthorisedCommand
    {
        public AddAuthorRequest(ClaimsPrincipal claims, int libraryId, AuthorModel author)
            : base(claims, libraryId)
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

        [Authorise(step: 1, HandlerTiming.Before)]
        public override async Task<AddAuthorRequest> HandleAsync(AddAuthorRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            command.Result = await _authorRepository.AddAuthor(command.LibraryId, command.Author, cancellationToken);

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
