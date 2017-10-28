using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Api.Renderers;
using Inshapardaz.Api.View;
using Inshapardaz.Domain.Database.Entities;
using Inshapardaz.Domain.Queries;
using Paramore.Brighter;
using Paramore.Darker;

namespace Inshapardaz.Api.Adapters.Dictionary
{
    public class GetMeaningForWordDetailRequest : IRequest
    {
        public Guid Id { get; set; }

        public List<MeaningView> Result { get; set; }

        public long DetailId { get; set; }

        public int DictionaryId { get; set; }
    }

    public class GetMeaningForWordDetailRequestHandler : RequestHandlerAsync<GetMeaningForWordDetailRequest>
    {
        private readonly IQueryProcessor _queryProcessor;
        private readonly IRenderMeaning _meaningRenderer;

        public GetMeaningForWordDetailRequestHandler(IQueryProcessor queryProcessor, IRenderMeaning meaningRenderer)
        {
            _queryProcessor = queryProcessor;
            _meaningRenderer = meaningRenderer;
        }

        public override async Task<GetMeaningForWordDetailRequest> HandleAsync(GetMeaningForWordDetailRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            //TODO :  Check for dictionary access
            IEnumerable<Meaning> meanings = await _queryProcessor.ExecuteAsync(new WordMeaningByWordDetailQuery
            {
                WordDetailId = command.DetailId
            }, cancellationToken);
            command.Result = meanings.Select(x => _meaningRenderer.Render(x)).ToList();
            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
