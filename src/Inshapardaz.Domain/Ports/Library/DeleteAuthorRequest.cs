using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Library
{
    public class DeleteAuthorRequest : RequestBase
    {
        public DeleteAuthorRequest(int authorId)
        {
            AuthorId = authorId;
        }

        public int AuthorId { get; }
    }

    public class DeleteAuthorRequestHandler : RequestHandlerAsync<DeleteAuthorRequest>
    {
        private readonly IAuthorRepository _authorRepository;

        public DeleteAuthorRequestHandler(IAuthorRepository authorRepository)
        {
            _authorRepository = authorRepository;
        }

        public override async Task<DeleteAuthorRequest> HandleAsync(DeleteAuthorRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            await _authorRepository.DeleteAuthor(command.AuthorId, cancellationToken);

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}