using System.Threading;
using System.Threading.Tasks;
using Darker;
using Inshapardaz.Domain.Commands;
using Inshapardaz.Domain.Database;
using Inshapardaz.Domain.Database.Entities;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Queries;
using paramore.brighter.commandprocessor;

namespace Inshapardaz.Domain.CommandHandlers
{
    public class AddWordRelationCommandHandler : RequestHandlerAsync<AddWordRelationCommand>
    {
        private readonly IDatabaseContext _database;
        private readonly IQueryProcessor _queryProcessor;

        public AddWordRelationCommandHandler(
            IDatabaseContext database,
            IQueryProcessor queryProcessor)
        {
            _database = database;
            _queryProcessor = queryProcessor;
        }

        public override async Task<AddWordRelationCommand> HandleAsync(AddWordRelationCommand command, CancellationToken cancellationToken = default(CancellationToken))
        {
            var word = await _queryProcessor.ExecuteAsync(new WordByIdQuery { Id = command.SourceWordId }, cancellationToken);
            if (word == null)
            {
                throw new RecordNotFoundException();
            }

            var relatedWord = await _queryProcessor.ExecuteAsync(new WordByIdQuery { Id = command.RelatedWordId }, cancellationToken);
            if (relatedWord == null)
            {
                throw new RecordNotFoundException();
            }

            var relation = new WordRelation
            {
                SourceWord = word,
                SourceWordId = command.SourceWordId,
                RelatedWord = relatedWord,
                RelatedWordId = command.RelatedWordId,
                RelationType = command.RelationType
            };

            _database.WordRelation.Add(relation);
            await _database.SaveChangesAsync(cancellationToken);

            return await base.HandleAsync(command, cancellationToken);
        }        
    }
}