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
    public class GetRelationshipsForWordRequest : IRequest
    {
        public Guid Id { get; set; }

        public List<RelationshipView> Result { get; set; }

        public int DictionaryId { get; set; }

        public long WordId { get; set; }
    }

    public class GetRelationshipsForWordRequestHandler : RequestHandlerAsync<GetRelationshipsForWordRequest>
    {
        private readonly IQueryProcessor _queryProcessor;
        private readonly IUserHelper _userHelper;
        private readonly IRenderRelation _relationRender;

        public GetRelationshipsForWordRequestHandler(IQueryProcessor queryProcessor, IUserHelper userHelper, IRenderRelation relationRender)
        {
            _queryProcessor = queryProcessor;
            _userHelper = userHelper;
            _relationRender = relationRender;
        }

        public override async Task<GetRelationshipsForWordRequest> HandleAsync(GetRelationshipsForWordRequest command, CancellationToken cancellationToken = new CancellationToken())
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
            
            var relations = await _queryProcessor.ExecuteAsync(new RelationshipByWordIdQuery { WordId = command.WordId }, cancellationToken);
            command.Result = relations.Select(r => _relationRender.Render(r)).ToList();
            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
