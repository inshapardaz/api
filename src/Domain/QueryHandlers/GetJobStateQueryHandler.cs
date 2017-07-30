using System.Collections.Generic;
using System.Linq;

using Inshapardaz.Domain.Queries;

using Darker;
using Inshapardaz.Domain.Model;
using System;
using System.Threading;
using System.Threading.Tasks;
using Hangfire.Storage;
using Microsoft.EntityFrameworkCore;

namespace Inshapardaz.Domain.QueryHandlers
{
    public class GetJobStateQueryHandler : AsyncQueryHandler<GetJobStateQuery, string>
    {
        private readonly IStorageConnection _storage;

        public GetJobStateQueryHandler(IStorageConnection storage)
        {
            _storage = storage;
        }

        public override async Task<string> ExecuteAsync(GetJobStateQuery query, CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }
    }
}