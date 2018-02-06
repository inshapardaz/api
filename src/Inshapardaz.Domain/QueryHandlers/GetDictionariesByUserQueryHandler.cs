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
using Nest;

namespace Inshapardaz.Domain.QueryHandlers
{
    public class GetDictionariesByUserQueryHandler : QueryHandlerAsync<GetDictionariesByUserQuery,
        IEnumerable<Dictionary>>
    {
        private readonly IClientProvider _clientProvider;

        public GetDictionariesByUserQueryHandler(IClientProvider clientProvider)
        {
            _clientProvider = clientProvider;
        }

        public override async Task<IEnumerable<Dictionary>> ExecuteAsync(GetDictionariesByUserQuery query,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var client = _clientProvider.GetClient();
            
            ISearchResponse<Dictionary> response = null;
            if (query.UserId != Guid.Empty)
            {
                response = await client.SearchAsync<Dictionary>(s => s
                                    .Index(Indexes.Dictionaries)
                                    .Size(100)
                                    .Query(q => q
                                    .Bool(b => b
                                        .Must(m => m
                                            .Term(term => term .Field(f => f.IsPublic).Value(true)),
                                             m => m.Term(r => r.Field(x => x.UserId).Value(query.UserId))
                                                )
                                    )), cancellationToken);
            }
            else
            {
                response = await client.SearchAsync<Dictionary>(s => s
                            .Index(Indexes.Dictionaries)
                            .Size(100)
                            .Query(q => q
                            .Bool(b => b
                                .Must(m => m
                                    .Term(term => term.Field(f => f.IsPublic).Value(true))
                                )
                            )), cancellationToken);
            }

            return response.Documents;
        }
    }
}