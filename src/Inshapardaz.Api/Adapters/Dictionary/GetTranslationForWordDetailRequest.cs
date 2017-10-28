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
    public class GetTranslationForWordDetailRequest : DictionaryRequest
    {
        public List<TranslationView> Result { get; set; }

        public long WordDetailId { get; set; }
    }

    public class GetTranslationForWordDetailRequestHandler : RequestHandlerAsync<GetTranslationForWordDetailRequest>
    {
        private readonly IQueryProcessor _queryProcessor;
        private readonly IRenderTranslation _translationRenderer;

        public GetTranslationForWordDetailRequestHandler(IQueryProcessor queryProcessor, IRenderTranslation translationRenderer)
        {
            _queryProcessor = queryProcessor;
            _translationRenderer = translationRenderer;
        }

        [DictionaryRequestValidation(1, HandlerTiming.Before)]
        public override async Task<GetTranslationForWordDetailRequest> HandleAsync(GetTranslationForWordDetailRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            var translations = await _queryProcessor.ExecuteAsync(new TranslationsByWordDetailIdQuery
            {
                WordDetailId = command.WordDetailId
            }, cancellationToken);
            command.Result =  translations.Select(t => _translationRenderer.Render(t, command.DictionaryId)).ToList();
            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
