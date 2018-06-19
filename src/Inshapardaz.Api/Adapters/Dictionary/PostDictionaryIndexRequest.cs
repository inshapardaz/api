using System;
using System.Threading;
using System.Threading.Tasks;
using Hangfire;
using Inshapardaz.Domain.Jobs;
using Inshapardaz.Domain.Queries;
using Paramore.Brighter;
using Paramore.Darker;

namespace Inshapardaz.Api.Adapters.Dictionary
{
    public class PostDictionaryIndexRequest : IRequest
    {
        public Guid Id { get; set; }
    }

    public class PostDictionaryIndexRequestHandler : RequestHandlerAsync<PostDictionaryIndexRequest>
    {
        private readonly IQueryProcessor _queryProcessor;
        private readonly IBackgroundJobClient _jobClient;

        public PostDictionaryIndexRequestHandler(IQueryProcessor queryProcessor, IBackgroundJobClient jobClient)
        {
            _queryProcessor = queryProcessor;
            _jobClient = jobClient;
        }
        
        public override async Task<PostDictionaryIndexRequest> HandleAsync(PostDictionaryIndexRequest command, CancellationToken cancellationToken = new CancellationToken())
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