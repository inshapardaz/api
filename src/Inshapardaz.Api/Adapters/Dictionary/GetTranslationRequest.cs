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
    public class GetTranslationRequest : DictionaryRequest
    {
        public GetTranslationRequest(int dictionaryId, long wordId, int translationId)
            : base(dictionaryId)
        {
            WordId = wordId;
            TranslationId = translationId;
        }

        public TranslationView Result { get; set; }

        public long WordId { get; }

        public long TranslationId { get; }
    }

    public class GetTranslationRequestHandler : RequestHandlerAsync<GetTranslationRequest>
    {
        private readonly IQueryProcessor _queryProcessor;
        private readonly IRenderTranslation _translationRenderer;

        public GetTranslationRequestHandler(IQueryProcessor queryProcessor, IRenderTranslation translationRenderer)
        {
            _queryProcessor = queryProcessor;
            _translationRenderer = translationRenderer;
        }

        [DictionaryRequestValidation(1, HandlerTiming.Before)]
        public override async Task<GetTranslationRequest> HandleAsync(GetTranslationRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            var translation = await _queryProcessor.ExecuteAsync(new GetTranslationByIdQuery(command.DictionaryId, command.WordId, command.TranslationId), cancellationToken);
            command.Result =  _translationRenderer.Render(translation, command.DictionaryId);
            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
