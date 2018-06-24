using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Repositories.Dictionary;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Dictionary
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
        private readonly IWordRepository _wordRepository;
        private readonly IRelationshipRepository _relationshipRepository;

        public DeleteWordRequestHandler(IWordRepository wordRepository, IRelationshipRepository relationshipRepository)
        {
            _wordRepository = wordRepository;
            _relationshipRepository = relationshipRepository;
        }

        [DictionaryWriteRequestValidation(1, HandlerTiming.Before)]
        public override async Task<DeleteWordRequest> HandleAsync(DeleteWordRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            var word = await _wordRepository.GetWordById(command.DictionaryId, command.WordId, cancellationToken);

            if (word != null)
            {
                await RemoveRelationships(command, cancellationToken);

                await _wordRepository.DeleteWord(command.DictionaryId, command.WordId, cancellationToken);
            }

            return await base.HandleAsync(command, cancellationToken);
        }

        private async Task RemoveRelationships(DeleteWordRequest command, CancellationToken cancellationToken)
        {
            var relationsFrom = await _relationshipRepository.GetRelationshipFromWord(command.DictionaryId, command.WordId, cancellationToken);
            var relationsTo = await _relationshipRepository.GetRelationshipToWord(command.DictionaryId, command.WordId, cancellationToken);

            var tasks = new List<Task>();
            foreach (var relation in relationsFrom)
            {
                tasks.Add(_relationshipRepository.DeleteRelationship(command.DictionaryId, relation.Id, cancellationToken));
            }

            foreach (var relation in relationsTo)
            {
                tasks.Add(_relationshipRepository.DeleteRelationship(command.DictionaryId, relation.Id, cancellationToken));
            }

            await Task.WhenAll(tasks);
        }
    }
}