using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Library
{
    public class AddAuthorRequest : RequestBase
    {
        public AddAuthorRequest(AuthorModel author)
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

        public override async Task<AddAuthorRequest> HandleAsync(AddAuthorRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            command.Result= await _authorRepository.AddAuthor(command.Author, cancellationToken);

            return await base.HandleAsync(command, cancellationToken);
        }
    } 
}
