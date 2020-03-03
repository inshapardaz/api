using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Domain.Ports.Handlers.Library;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Brighter;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Library
{
    public class UpdateAuthorRequest : LibraryAuthorisedCommand
    {
        public UpdateAuthorRequest(ClaimsPrincipal claims, int libraryId, AuthorModel author)
            : base(claims, libraryId)
        {
            Author = author;
        }

        public AuthorModel Author { get; }

        public RequestResult Result { get; set; } = new RequestResult();

        public class RequestResult
        {
            public AuthorModel Author { get; set; }

            public bool HasAddedNew { get; set; }
        }
    }

    public class UpdateAuthorRequestHandler : RequestHandlerAsync<UpdateAuthorRequest>
    {
        private readonly IAuthorRepository _authorRepository;

        public UpdateAuthorRequestHandler(IAuthorRepository authorRepository)
        {
            _authorRepository = authorRepository;
        }

        [Authorise(step: 1, HandlerTiming.Before)]
        public override async Task<UpdateAuthorRequest> HandleAsync(UpdateAuthorRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            var result = await _authorRepository.GetAuthorById(command.LibraryId, command.Author.Id, cancellationToken);

            if (result == null)
            {
                var author = command.Author;
                author.Id = default(int);
                command.Result.Author = await _authorRepository.AddAuthor(command.LibraryId, author, cancellationToken);
                command.Result.HasAddedNew = true;
            }
            else
            {
                await _authorRepository.UpdateAuthor(command.LibraryId, command.Author, cancellationToken);
                command.Result.Author = command.Author;
            }

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
