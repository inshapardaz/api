﻿using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Commands;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Queries;
using Paramore.Brighter;
using Paramore.Darker;

namespace Inshapardaz.Api.Adapters.Dictionary
{
    public class DeleteWordRequest : DictionaryRequest
    {
        public DeleteWordRequest(int dictionaryId)
            : base(dictionaryId)
        {
        }

        public int WordId { get; set; }
    }

    public class DeleteWordRequestHandler : RequestHandlerAsync<DeleteWordRequest>
    {
        private readonly IQueryProcessor _queryProcessor;
        private readonly IAmACommandProcessor _commandProcessor;

        public DeleteWordRequestHandler(IAmACommandProcessor commandProcessor, IQueryProcessor queryProcessor)
        {
            _commandProcessor = commandProcessor;
            _queryProcessor = queryProcessor;
        }

        [DictionaryRequestValidation(1, HandlerTiming.Before)]
        public override async Task<DeleteWordRequest> HandleAsync(DeleteWordRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            var word = await _queryProcessor.ExecuteAsync(new GetWordByIdQuery(command.DictionaryId, command.WordId), cancellationToken);

            if (word == null)
            {
                throw new NotFoundException();
            }

            await RemoveRelationships(command, cancellationToken);

            await _commandProcessor.SendAsync(new DeleteWordCommand(command.DictionaryId, command.WordId), cancellationToken: cancellationToken);

            return await base.HandleAsync(command, cancellationToken);
        }

        private async Task RemoveRelationships(DeleteWordRequest command, CancellationToken cancellationToken)
        {
            var relations = await _queryProcessor.ExecuteAsync(new GetRelationshipsByWordQuery(command.WordId), cancellationToken);
            var relationTo = await _queryProcessor.ExecuteAsync(new GetRelationshipToWordQuery(command.WordId), cancellationToken);

            foreach (var relation in relations.Union(relationTo))
            {
                await _commandProcessor.SendAsync(new DeleteRelationshipRequest(command.DictionaryId) { RelationshipId = relation.Id });
            }
        }
    }
}