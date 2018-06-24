using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Api.Renderers;
using Inshapardaz.Api.Renderers.Dictionary;
using Inshapardaz.Api.View;
using Inshapardaz.Api.View.Dictionary;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Queries;
using Inshapardaz.Domain.Queries.Dictionary;
using Paramore.Brighter;
using Paramore.Darker;

namespace Inshapardaz.Api.Adapters.Dictionary
{
    public class GetWordByIdRequest : DictionaryRequest
    {
        public GetWordByIdRequest(int dictionaryId)
            : base(dictionaryId)
        {
        }

        public long WordId { get; set; }

        public WordView Result { get; set; }
    }

    public class GetWordByIdRequestHandler : RequestHandlerAsync<GetWordByIdRequest>
    {
        private readonly IQueryProcessor _queryProcessor;
        private readonly IRenderWord _wordRenderer;

        public GetWordByIdRequestHandler(IQueryProcessor queryProcessor, IRenderWord wordRenderer)
        {
            _queryProcessor = queryProcessor;
            _wordRenderer = wordRenderer;
        }

        [DictionaryRequestValidation(1, HandlerTiming.Before)]
        public override async Task<GetWordByIdRequest> HandleAsync(GetWordByIdRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            var word = await _queryProcessor.ExecuteAsync(new GetWordByIdQuery(command.DictionaryId, command.WordId), cancellationToken);
            if (word == null)
            {
                throw new NotFoundException();
            }

            command.Result = _wordRenderer.Render(word, command.DictionaryId);
            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
