using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Api.Helpers;
using Inshapardaz.Api.Renderers;
using Inshapardaz.Api.Renderers.Dictionary;
using Inshapardaz.Api.View;
using Inshapardaz.Api.View.Dictionary;
using Inshapardaz.Domain.Entities;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Helpers;
using Inshapardaz.Domain.Queries;
using Inshapardaz.Domain.Queries.Dictionary;
using Paramore.Brighter;
using Paramore.Darker;

namespace Inshapardaz.Api.Adapters.Dictionary
{
    public class GetDictionaryByIdRequest : DictionaryRequest
    {
        public GetDictionaryByIdRequest(int dictionaryId)
            : base(dictionaryId)
        {
        }

        public DictionaryView Result { get; set; }
    }

    public class GetDictionaryByIdRequestHandler : RequestHandlerAsync<GetDictionaryByIdRequest>
    {
        private readonly IRenderDictionary _dictionaryRenderer;
        private readonly IUserHelper _userHelper;
        private readonly IQueryProcessor _queryProcessor;

        public GetDictionaryByIdRequestHandler(IQueryProcessor queryProcessor, 
                                               IRenderDictionary dictionaryRenderer,
                                               IUserHelper userHelper)
        {
            _queryProcessor = queryProcessor;
            _dictionaryRenderer = dictionaryRenderer;
            _userHelper = userHelper;
        }

        [DictionaryRequestValidation(1, HandlerTiming.Before)]
        public override async Task<GetDictionaryByIdRequest> HandleAsync(GetDictionaryByIdRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            var result = await _queryProcessor.ExecuteAsync(new GetDictionaryByIdQuery { DictionaryId = command.DictionaryId }, cancellationToken);

            if (result == null)
            {
                throw new NotFoundException();
            }

            var wordCount = await _queryProcessor.ExecuteAsync(new GetDictionaryWordCountQuery {DictionaryId = command.DictionaryId}, cancellationToken);
            command.Result = _dictionaryRenderer.Render(result, wordCount);
            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
