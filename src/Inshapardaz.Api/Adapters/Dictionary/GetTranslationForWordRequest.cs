using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Api.Helpers;
using Inshapardaz.Api.Renderers;
using Inshapardaz.Api.View;
using Inshapardaz.Domain.Queries;
using Paramore.Brighter;
using Paramore.Darker;

namespace Inshapardaz.Api.Adapters.Dictionary
{
    public class GetTranslationForWordRequest : IRequest
    {
        public Guid Id { get; set; }

        public List<TranslationView> Result { get; set; }

        public int DictionaryId { get; set; }

        public long WordId { get; set; }
    }

    public class GetTranslationForWordRequestHandler : RequestHandlerAsync<GetTranslationForWordRequest>
    {
        private readonly IQueryProcessor _queryProcessor;
        private readonly IUserHelper _userHelper;
        private readonly IRenderTranslation _translationRenderer;

        public GetTranslationForWordRequestHandler(IQueryProcessor queryProcessor, IUserHelper userHelper, IRenderTranslation translationRenderer)
        {
            _queryProcessor = queryProcessor;
            _userHelper = userHelper;
            _translationRenderer = translationRenderer;
        }

        public override async Task<GetTranslationForWordRequest> HandleAsync(GetTranslationForWordRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            var user = _userHelper.GetUserId();
            if (user != Guid.Empty)
            {
                var dictionary = await _queryProcessor.ExecuteAsync(new DictionaryByWordIdQuery
                {
                    WordId = command.WordId
                }, cancellationToken);
                if (dictionary != null && dictionary.UserId != user)
                {
                    throw new UnauthorizedAccessException();
                }
            }

            var translations = await _queryProcessor.ExecuteAsync(new TranslationsByWordIdQuery
            {
                WordId = command.WordId
            }, cancellationToken);
            command.Result =  translations.Select(t => _translationRenderer.Render(t)).ToList();
            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
