using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Api.Renderers;
using Inshapardaz.Api.Renderers.Dictionary;
using Inshapardaz.Api.View;
using Inshapardaz.Api.View.Dictionary;
using Inshapardaz.Domain.Queries;
using Inshapardaz.Domain.Queries.Dictionary;
using Paramore.Brighter;
using Paramore.Darker;

namespace Inshapardaz.Api.Adapters.Dictionary
{
    public class GetMeaningForContextRequest : DictionaryRequest
    {
        public GetMeaningForContextRequest(int dictionaryId, long wordId, string context)
            : base(dictionaryId)
        {
            WordId = wordId;
            Context = context;
        }

        public string Context { get; set; }

        public long WordId { get; set; }

        public IEnumerable<MeaningView> Result { get; set; }
    }

    public class GetMeaningForContextRequestHandler : RequestHandlerAsync<GetMeaningForContextRequest>
    {
        private readonly IQueryProcessor _queryProcessor;
        private readonly IRenderMeaning _meaningRenderer;

        public GetMeaningForContextRequestHandler(IQueryProcessor queryProcessor, IRenderMeaning meaningRenderer)
        {
            _queryProcessor = queryProcessor;
            _meaningRenderer = meaningRenderer;
        }

        [DictionaryRequestValidation(1, HandlerTiming.Before)]
        public override async Task<GetMeaningForContextRequest> HandleAsync(GetMeaningForContextRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            var finalContext = string.Empty;
            if (command.Context != "default")
            {
                finalContext = command.Context;
            }

            var result = await _queryProcessor.ExecuteAsync(new GetWordMeaningsByContextQuery(command.DictionaryId, command.WordId, finalContext), cancellationToken);

            command.Result = result.Select(x => _meaningRenderer.Render(x, command.DictionaryId)).ToList();
            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
