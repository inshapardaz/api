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
    public class GetMeaningForWordRequest : DictionaryRequest
    {
        public GetMeaningForWordRequest(int dictionaryId)
            : base(dictionaryId)
        {
        }

        public long WordId { get; set; }

        public List<MeaningView> Result { get; set; }
    }

    public class GetMeaningForWordRequestHandler : RequestHandlerAsync<GetMeaningForWordRequest>
    {
        private readonly IQueryProcessor _queryProcessor;
        private readonly IRenderMeaning _meaningRenderer;

        public GetMeaningForWordRequestHandler(IQueryProcessor queryProcessor, IRenderMeaning meaningRenderer)
        {
            _queryProcessor = queryProcessor;
            _meaningRenderer = meaningRenderer;
        }

        [DictionaryRequestValidation(1, HandlerTiming.Before)]
        public override async Task<GetMeaningForWordRequest> HandleAsync(GetMeaningForWordRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            var meanings = await _queryProcessor.ExecuteAsync(new GetWordMeaningsByWordQuery(command.DictionaryId, command.WordId), cancellationToken);
            command.Result = meanings.Select(x => _meaningRenderer.Render(x, command.DictionaryId)).ToList();
            return await base.HandleAsync(command, cancellationToken);
        }
    }
}