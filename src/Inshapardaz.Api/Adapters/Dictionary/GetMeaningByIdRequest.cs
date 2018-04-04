using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Api.Renderers;
using Inshapardaz.Api.View;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Queries;
using Paramore.Brighter;
using Paramore.Darker;

namespace Inshapardaz.Api.Adapters.Dictionary
{
    public class GetMeaningByIdRequest : DictionaryRequest
    {
        public GetMeaningByIdRequest(int dictionaryId, long wordId, long meaningId)
            : base(dictionaryId)
        {
            MeaningId = meaningId;
            WordId = wordId;
        }

        public long MeaningId { get; }

        public long WordId { get; }

        public MeaningView Result { get; set;  }
    }
    
    public class GetMeaningByIdRequestHandler : RequestHandlerAsync<GetMeaningByIdRequest>
    {
        private readonly IQueryProcessor _queryProcessor;
        private readonly IRenderMeaning _meaningRenderer;

        public GetMeaningByIdRequestHandler(IQueryProcessor queryProcessor, IRenderMeaning meaningRenderer)
        {
            _queryProcessor = queryProcessor;
            _meaningRenderer = meaningRenderer;
        }

        [DictionaryRequestValidation(1, HandlerTiming.Before)]
        public override async Task<GetMeaningByIdRequest> HandleAsync(GetMeaningByIdRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            var meaning = await _queryProcessor.ExecuteAsync(new GetWordMeaningByIdQuery(command.DictionaryId, command.WordId, command.MeaningId), cancellationToken);

            if (meaning == null)
            {
                throw new NotFoundException();
            }

            command.Result = _meaningRenderer.Render(meaning, command.DictionaryId);
            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
