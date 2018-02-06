using System;
using System.Collections.Generic;
using System.Linq;
using Inshapardaz.Domain.Queries;
using Paramore.Darker;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Database;
using Inshapardaz.Domain.Database.Entities;
using Inshapardaz.Domain.Elasticsearch;
using Microsoft.EntityFrameworkCore;

namespace Inshapardaz.Domain.QueryHandlers
{
    public class GetDictionariesQueryHandler : QueryHandlerAsync<GetDictionariesQuery,
        IEnumerable<Dictionary>>
    {
        private readonly IClientProvider _clientProvider;

        public GetDictionariesQueryHandler(IClientProvider clientProvider)
        {
            _clientProvider = clientProvider;
        }

        public override async Task<IEnumerable<Dictionary>> ExecuteAsync(GetDictionariesQuery query,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var client = _clientProvider.GetClient();
            var response = await client.SearchAsync<Dictionary>(s => s.Query(q => q.MatchAll()), cancellationToken);

            return response.Documents;
        }
    }
}