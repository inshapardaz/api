using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Library
{
    public class UpdateAuthorRequest : RequestBase
    {
        public UpdateAuthorRequest(AuthorModel author)
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

        public override async Task<UpdateAuthorRequest> HandleAsync(UpdateAuthorRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            var result = await _authorRepository.GetAuthorById(command.Author.Id, cancellationToken);

            if (result == null)
            {
                var author = command.Author;
                author.Id = default(int);
                command.Result.Author =  await  _authorRepository.AddAuthor(author, cancellationToken);
                command.Result.HasAddedNew = true;
            }
            else
            {
                await _authorRepository.UpdateAuthor(command.Author, cancellationToken);
                command.Result.Author = command.Author;
            }

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}