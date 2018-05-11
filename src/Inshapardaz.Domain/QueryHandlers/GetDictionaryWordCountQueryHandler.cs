using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Queries;
using Paramore.Darker;

namespace Inshapardaz.Domain.QueryHandlers
{
    public class GetDictionaryWordCountQueryHandler : QueryHandlerAsync<GetDictionaryWordCountQuery, int>
    {
        //private readonly IClientProvider _clientProvider;
        //private readonly IProvideIndex _indexProvider;

        //public GetDictionaryWordCountQueryHandler(IClientProvider clientProvider, IProvideIndex indexProvider)
        //{
        //    _clientProvider = clientProvider;
        //    _indexProvider = indexProvider;
        //}

        //public override async Task<int> ExecuteAsync(GetDictionaryWordCountQuery query,
        //    CancellationToken cancellationToken = default(CancellationToken))
        //{
        //    ;
        //    var client = _clientProvider.GetClient();
        //    var indexName = _indexProvider.GetIndexForDictionary(query.DictionaryId);

        //    var result = await client.CountAsync<Word>(s => s.Index(indexName), cancellationToken);

        //    return (int) result.Count;

        //}
        public override Task<int> ExecuteAsync(GetDictionaryWordCountQuery query, CancellationToken cancellationToken = new CancellationToken())
        {
            throw new System.NotImplementedException();
        }
    }
}