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
    public class GetTranslationForWordDetailLanguageRequest : DictionaryRequest
    {
        public List<TranslationView> Result { get; set; }

        public long WordDetailId { get; set; }

        public Languages Language { get; set; }
    }

    public class GetTranslationForWordDetailLanguageRequestHandler : RequestHandlerAsync<GetTranslationForWordDetailLanguageRequest>
    {
        private readonly IQueryProcessor _queryProcessor;
        private readonly IRenderTranslation _translationRenderer;

        public GetTranslationForWordDetailLanguageRequestHandler(IQueryProcessor queryProcessor, IRenderTranslation translationRenderer)
        {
            _queryProcessor = queryProcessor;
            _translationRenderer = translationRenderer;
        }

        [DictionaryRequestValidation(1, HandlerTiming.Before)]
        public override async Task<GetTranslationForWordDetailLanguageRequest> HandleAsync(GetTranslationForWordDetailLanguageRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            var translations = await _queryProcessor.ExecuteAsync(new TranslationsByWordDetailAndLanguageQuery
            {
                WordDetailId = command.WordDetailId,
                Language = command.Language
            }, cancellationToken);

            command.Result = translations.Select(t => _translationRenderer.Render(t)).ToList();
            return await base.HandleAsync(command, cancellationToken);
        }
    }
}