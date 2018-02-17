using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Elasticsearch;
using Inshapardaz.Domain.Entities;
using Inshapardaz.Domain.Queries;
using Paramore.Darker;

namespace Inshapardaz.Domain.QueryHandlers
{
    public class GetDictionaryDownloadsQueryHandler : QueryHandlerAsync<GetDictionaryDownloadsQuery, IEnumerable<DictionaryDownload>>
    {
        private readonly IClientProvider _clientProvider;

        public GetDictionaryDownloadsQueryHandler(IClientProvider clientProvider)
        {
            _clientProvider = clientProvider;
        }

        public override async Task<IEnumerable<DictionaryDownload>> ExecuteAsync(GetDictionaryDownloadsQuery query,
                                                      CancellationToken cancellationToken = default(CancellationToken))
        {
            var client = _clientProvider.GetClient();

            var response = await client.SearchAsync<Dictionary>(s => s
                                        .Index(Indexes.Dictionaries)
                                        .Size(1)
                                        .Query(q => q
                                        .Bool(b => b
                                        .Must(m => m
                                        .Term(term => term.Field(f => f.Id).Value(query.DictionaryId)))
                                    )), cancellationToken);

            var dictionary = response.Documents.SingleOrDefault();

            if (dictionary == null)
                return new DictionaryDownload[0];

            if (dictionary.IsPublic || query.UserId == dictionary.UserId )
            {
                return dictionary.Downloads;
            }

            return new DictionaryDownload[0];
        }
    }
}