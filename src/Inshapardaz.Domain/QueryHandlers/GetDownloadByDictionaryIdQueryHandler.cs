using System;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Entities;
using Inshapardaz.Domain.Queries;
using Paramore.Darker;

namespace Inshapardaz.Domain.QueryHandlers
{
    public class GetDownloadByDictionaryIdQueryHandler : QueryHandlerAsync<GetDownloadByDictionaryIdQuery, File>
    {
        //private readonly IClientProvider _clientProvider;

        //public GetDownloadByDictionaryIdQueryHandler(IClientProvider clientProvider)
        //{
        //    _clientProvider = clientProvider;
        //}

        public override async Task<File> ExecuteAsync(GetDownloadByDictionaryIdQuery query,
                                                            CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }
    }
}