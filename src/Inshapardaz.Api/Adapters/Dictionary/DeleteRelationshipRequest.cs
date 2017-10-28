using System;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Api.Helpers;
using Inshapardaz.Domain.Commands;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Queries;
using Paramore.Brighter;
using Paramore.Darker;

namespace Inshapardaz.Api.Adapters.Dictionary
{
    public class DeleteRelationshipRequest : IRequest
    {
        public Guid Id { get; set; }

        public long RelationshipId { get; set; }

        public int DictionaryId { get; set; }
    }

    public class DeleteRelationshipRequestHandler : RequestHandlerAsync<DeleteRelationshipRequest>
    {
        private readonly IAmACommandProcessor _commandProcessor;
        private readonly IQueryProcessor _queryProcessor;
        private readonly IUserHelper _userHelper;

        public DeleteRelationshipRequestHandler(IAmACommandProcessor commandProcessor, IQueryProcessor queryProcessor, IUserHelper userHelper)
        {
            _commandProcessor = commandProcessor;
            _queryProcessor = queryProcessor;
            _userHelper = userHelper;
        }

        public override async Task<DeleteRelationshipRequest> HandleAsync(DeleteRelationshipRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            var relations = await _queryProcessor.ExecuteAsync(new RelationshipByIdQuery { Id = command.RelationshipId }, cancellationToken);

            if (relations == null)
            {
                throw new NotFoundException();
            }

            var dictionary = await _queryProcessor.ExecuteAsync(new DictionaryByWordIdQuery { WordId = (int)relations.SourceWordId }, cancellationToken);
            if (dictionary == null || dictionary.UserId != _userHelper.GetUserId())
            {
                throw new UnauthorizedAccessException();
            }

            await _commandProcessor.SendAsync(new DeleteWordRelationCommand { RelationId = command.RelationshipId }, cancellationToken: cancellationToken);
            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
