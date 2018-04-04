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
    public class GetRelationshipRequest : DictionaryRequest
    {
        public GetRelationshipRequest(int dictionaryId, long wordId, int relationId)
            : base(dictionaryId)
        {
            WordId = wordId;
            RelationId = relationId;
        }

        public RelationshipView Result { get; set; }

        public long WordId { get; }

        public int RelationId { get; set; }
    }

    public class GetRelationshipRequestHandler : RequestHandlerAsync<GetRelationshipRequest>
    {
        private readonly IQueryProcessor _queryProcessor;
        private readonly IRenderRelation _relationRender;

        public GetRelationshipRequestHandler(IQueryProcessor queryProcessor, IRenderRelation relationRender)
        {
            _queryProcessor = queryProcessor;
            _relationRender = relationRender;
        }

        [DictionaryRequestValidation(1, HandlerTiming.Before)]
        public override async Task<GetRelationshipRequest> HandleAsync(GetRelationshipRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            var relations = await _queryProcessor.ExecuteAsync(new GetRelationshipByIdQuery(command.DictionaryId, command.WordId, command.RelationId), cancellationToken);

            if (relations == null)
            {
                throw new BadRequestException();
            }

            command.Result = _relationRender.Render(relations, command.DictionaryId, command.WordId);
            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
