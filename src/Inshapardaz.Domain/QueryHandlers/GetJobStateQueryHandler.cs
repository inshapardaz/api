using Inshapardaz.Domain.Queries;

using Paramore.Darker;
using System;
using System.Threading;
using System.Threading.Tasks;
using Hangfire.Storage;

namespace Inshapardaz.Domain.QueryHandlers
{
    public class GetJobStateQueryHandler : QueryHandlerAsync<GetJobStateQuery, string>
    {
        private readonly IStorageConnection _storage;

        public GetJobStateQueryHandler(IStorageConnection storage)
        {
            _storage = storage;
        }

        public override Task<string> ExecuteAsync(GetJobStateQuery query, CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }
    }
}