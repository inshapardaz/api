using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Api.Renderers;
using Inshapardaz.Api.View;
using Inshapardaz.Domain.Queries;
using Paramore.Brighter;
using Paramore.Darker;

namespace Inshapardaz.Api.Adapters.Dictionary
{
    public class GetMeaningForWordDetailRequest : DictionaryRequest
    {
        public List<MeaningView> Result { get; set; }

        public long DetailId { get; set; }
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

        [DictionaryRequestValidation(1, HandlerTiming.Before)]
        public override async Task<GetMeaningForWordDetailRequest> HandleAsync(GetMeaningForWordDetailRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            var meanings = await _queryProcessor.ExecuteAsync(new WordMeaningByWordDetailQuery
            {
                WordDetailId = command.DetailId
            }, cancellationToken);
            command.Result = meanings.Select(x => _meaningRenderer.Render(x, command.DictionaryId)).ToList();
            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
