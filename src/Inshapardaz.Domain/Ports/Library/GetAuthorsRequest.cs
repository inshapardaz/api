using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Entities;
using Inshapardaz.Domain.Entities.Library;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Library
{
    public class GetAuthorsRequest : RequestBase
    {
        public GetAuthorsRequest(int pageNumber, int pageSize)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
        }

        public int PageNumber { get; private set; }

        public int PageSize { get; private set; }

        public Page<Author> Result { get; set; }
    }

    public class GetAuthorsRequestHandler : RequestHandlerAsync<GetAuthorsRequest>
    {
        private readonly IAuthorRepository _authorRepository;

        public GetAuthorsRequestHandler(IAuthorRepository authorRepository)
        {
            _authorRepository = authorRepository;
        }

        public override async Task<GetAuthorsRequest> HandleAsync(GetAuthorsRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            command.Result = await _authorRepository.GetAuthors(command.PageNumber, command.PageSize, cancellationToken);
            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
