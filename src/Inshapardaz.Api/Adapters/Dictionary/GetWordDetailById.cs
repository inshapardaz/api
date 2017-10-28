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
    public class GetWordDetailByIdRequest : DictionaryRequest
    {
        public long DetailId { get; set; }

        public WordDetailView Result { get; set; }
    }

    public class GetWordDetailByIdRequestHandler : RequestHandlerAsync<GetWordDetailByIdRequest>
    {
        private readonly IQueryProcessor _queryProcessor;
        private readonly IRenderWordDetail _wordDetailRenderer;

        public GetWordDetailByIdRequestHandler(IQueryProcessor queryProcessor, IRenderWordDetail wordDetailRenderer)
        {
            _queryProcessor = queryProcessor;
            _wordDetailRenderer = wordDetailRenderer;
        }

        [DictionaryRequestValidation(1, HandlerTiming.Before)]
        public override async Task<GetWordDetailByIdRequest> HandleAsync(GetWordDetailByIdRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            var details = await _queryProcessor.ExecuteAsync(new WordDetailByIdQuery { DictionaryId = command.DictionaryId, WordDetailId = command.DetailId }, cancellationToken);

            if (details == null)
            {
                throw new NotFoundException();
            }

            command.Result = _wordDetailRenderer.Render(details);
            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
