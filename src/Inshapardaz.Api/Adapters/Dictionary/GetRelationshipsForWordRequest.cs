using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Api.Renderers;
using Inshapardaz.Api.Renderers.Dictionary;
using Inshapardaz.Api.View;
using Inshapardaz.Api.View.Dictionary;
using Inshapardaz.Domain.Queries;
using Inshapardaz.Domain.Queries.Dictionary;
using Paramore.Brighter;
using Paramore.Darker;

namespace Inshapardaz.Api.Adapters.Dictionary
{
    public class GetRelationshipsForWordRequest : DictionaryRequest
    {
        public GetRelationshipsForWordRequest(int dictionaryId)
            : base(dictionaryId)
        {
        }

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
            var query = new GetRelationshipsByWordQuery(command.DictionaryId, command.WordId);
            var relations = await _queryProcessor.ExecuteAsync(query, cancellationToken);
            command.Result = relations.Select(r => _relationRender.Render(r, command.DictionaryId)).ToList();
            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
