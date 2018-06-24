using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Entities;
using Inshapardaz.Domain.Entities.Library;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Darker;

namespace Inshapardaz.Domain.Queries.Library
{
    public class GetAuthorsQuery : IQuery<Page<Author>>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }

    public class GetAuthorsQueryHandler : QueryHandlerAsync<GetAuthorsQuery, Page<Author>>
    {
        private readonly IAuthorRepository _authorRepository;

        public GetAuthorsQueryHandler(IAuthorRepository authorRepository)
        {
            _authorRepository = authorRepository;
        }
        public override async Task<Page<Author>> ExecuteAsync(GetAuthorsQuery query, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await _authorRepository.GetAuthors(query.PageNumber, query.PageSize, cancellationToken);
        }
    }
}
