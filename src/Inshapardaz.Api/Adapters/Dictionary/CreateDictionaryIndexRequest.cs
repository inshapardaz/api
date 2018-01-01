using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Hangfire;
using Inshapardaz.Api.Model;
using Inshapardaz.Api.Renderers;
using Inshapardaz.Api.View;
using Inshapardaz.Domain.Commands;
using Inshapardaz.Domain.Database.Entities;
using Inshapardaz.Domain.Jobs;
using Inshapardaz.Domain.Queries;
using Paramore.Brighter;
using Paramore.Darker;

namespace Inshapardaz.Api.Adapters.Dictionary
{
    public class CreateDictionaryIndexRequest : IRequest
    {
        public Guid Id { get; set; }
    }

    public class CreateDictionaryIndexRequestHandler : RequestHandlerAsync<CreateDictionaryIndexRequest>
    {
        private readonly IQueryProcessor _queryProcessor;
        private readonly IBackgroundJobClient _jobClient;

        public CreateDictionaryIndexRequestHandler(IQueryProcessor queryProcessor, IBackgroundJobClient jobClient)
        {
            _queryProcessor = queryProcessor;
            _jobClient = jobClient;
        }
        
        public override async Task<CreateDictionaryIndexRequest> HandleAsync(CreateDictionaryIndexRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            var dictionaries = await _queryProcessor.ExecuteAsync(new GetDictionariesQuery(), cancellationToken);
            foreach (var dictionary in dictionaries)
            {
                _jobClient.Enqueue<CreateDictionaryIndexJob>(j => j.CreateIndex(dictionary.Id));
            }

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}