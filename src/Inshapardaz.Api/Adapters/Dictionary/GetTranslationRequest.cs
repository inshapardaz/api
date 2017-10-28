using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Api.Renderers;
using Inshapardaz.Api.View;
using Inshapardaz.Domain.Queries;
using Paramore.Brighter;
using Paramore.Darker;

namespace Inshapardaz.Api.Adapters.Dictionary
{
    public class GetTranslationRequest : DictionaryRequest
    {
        public TranslationView Result { get; set; }

        public long TranslationId { get; set; }
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
            var translation = await _queryProcessor.ExecuteAsync(new TranslationByIdQuery
            {
                Id = command.TranslationId
            }, cancellationToken);
            command.Result =  _translationRenderer.Render(translation);
            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
