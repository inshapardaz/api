using System;
using System.Threading;
using System.Threading.Tasks;
using Hangfire.Storage;
using Paramore.Darker;

namespace Inshapardaz.Domain.Queries
{
    public class GetJobStateQuery : IQuery<string>
    {
        public string JobId { get; set; }
    }

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