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
    public class GetDictionaryByIdRequest : DictionaryRequest
    {
        public DictionaryView Result { get; set; }
    }

    public class GetDictionaryByIdRequestHandler : RequestHandlerAsync<GetDictionaryByIdRequest>
    {
        private readonly IRenderDictionary _dictionaryRenderer;
        private readonly IQueryProcessor _queryProcessor;

        public GetDictionaryByIdRequestHandler(IQueryProcessor queryProcessor, 
                                               IRenderDictionary dictionaryRenderer)
        {
            _queryProcessor = queryProcessor;
            _dictionaryRenderer = dictionaryRenderer;
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
