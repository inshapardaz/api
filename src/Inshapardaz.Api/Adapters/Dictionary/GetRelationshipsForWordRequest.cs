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
    public class GetRelationshipsForWordRequest : DictionaryRequest
    {
        public List<RelationshipView> Result { get; set; }

        public long WordId { get; set; }
    }

    public class GetRelationshipsForWordRequestHandler : RequestHandlerAsync<GetRelationshipsForWordRequest>
    {
        private readonly IQueryProcessor _queryProcessor;
        private readonly IRenderRelation _relationRender;

        public GetRelationshipsForWordRequestHandler(IQueryProcessor queryProcessor, IRenderRelation relationRender)
        {
            _queryProcessor = queryProcessor;
            _relationRender = relationRender;
        }

        [DictionaryRequestValidation(1, HandlerTiming.Before)]
        public override async Task<GetRelationshipsForWordRequest> HandleAsync(GetRelationshipsForWordRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            var relations = await _queryProcessor.ExecuteAsync(new GetRelationshipByWordIdQuery { WordId = command.WordId }, cancellationToken);
            command.Result = relations.Select(r => _relationRender.Render(r, command.DictionaryId)).ToList();
            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
