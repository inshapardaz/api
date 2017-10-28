using System;
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
    public class GetTranslationRequest : IRequest
    {
        public Guid Id { get; set; }

        public TranslationView Result { get; set; }

        public int DictionaryId { get; set; } 

        public long TranslationId { get; set; }
    }

    public class GetTranslationRequestHandler : RequestHandlerAsync<GetTranslationRequest>
    {
        private readonly IQueryProcessor _queryProcessor;
        private readonly IUserHelper _userHelper;
        private readonly IRenderTranslation _translationRenderer;

        public GetTranslationRequestHandler(IQueryProcessor queryProcessor, IUserHelper userHelper, IRenderTranslation translationRenderer)
        {
            _queryProcessor = queryProcessor;
            _userHelper = userHelper;
            _translationRenderer = translationRenderer;
        }

        public override async Task<GetTranslationRequest> HandleAsync(GetTranslationRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            var user = _userHelper.GetUserId();
            if (user != Guid.Empty)
            {
                var dictionary = await _queryProcessor.ExecuteAsync(new DictionaryByIdQuery
                {
                    DictionaryId = command.DictionaryId
                }, cancellationToken);
                if (dictionary != null && dictionary.UserId != user)
                {
                    throw new UnauthorizedAccessException();
                }
            }

            var translation = await _queryProcessor.ExecuteAsync(new TranslationByIdQuery
            {
                Id = command.TranslationId
            }, cancellationToken);
            command.Result =  _translationRenderer.Render(translation);
            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
