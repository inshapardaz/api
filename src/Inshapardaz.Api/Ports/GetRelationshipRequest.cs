using System;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Api.Helpers;
using Inshapardaz.Api.Renderers;
using Inshapardaz.Api.View;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Queries;
using Paramore.Brighter;
using Paramore.Darker;

namespace Inshapardaz.Api.Ports
{
    public class GetRelationshipRequest : IRequest
    {
        public Guid Id { get; set; }

        public RelationshipView Result { get; set; }

        public int DictionaryId { get; set; } 

        public long RelationId { get; set; }
    }

    public class GetRelationshipRequestHandler : RequestHandlerAsync<GetRelationshipRequest>
    {
        private readonly IQueryProcessor _queryProcessor;
        private readonly IUserHelper _userHelper;
        private readonly IRenderRelation _relationRender;

        public GetRelationshipRequestHandler(IQueryProcessor queryProcessor, IUserHelper userHelper, IRenderRelation relationRender)
        {
            _queryProcessor = queryProcessor;
            _userHelper = userHelper;
            _relationRender = relationRender;
        }

        public override async Task<GetRelationshipRequest> HandleAsync(GetRelationshipRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
             if (_userHelper.GetUserId() != Guid.Empty)
            {
                var dictionary = await _queryProcessor.ExecuteAsync(new DictionaryByWordIdQuery { WordId = command.RelationId }, cancellationToken);
                if (dictionary == null || dictionary.UserId != _userHelper.GetUserId())
                {
                    throw new UnauthorizedAccessException();
                }
            }

            var relations = await _queryProcessor.ExecuteAsync(new RelationshipByIdQuery { Id = command.RelationId }, cancellationToken);

            if (relations == null)
            {
                throw new BadRequestException();
            }

            command.Result = _relationRender.Render(relations);
            return command;
        }
    }
}
