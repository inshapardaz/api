using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Commands;
using Inshapardaz.Domain.Database;
using Inshapardaz.Domain.Database.Entities;
using Inshapardaz.Domain.Exception;
using Microsoft.EntityFrameworkCore;
using Paramore.Brighter;

namespace Inshapardaz.Domain.CommandHandlers
{
    public class AddWordRelationCommandHandler : RequestHandlerAsync<AddWordRelationCommand>
    {
        private readonly IDatabaseContext _database;

        public AddWordRelationCommandHandler(IDatabaseContext database)
        {
            _database = database;
        }

        public override async Task<AddWordRelationCommand> HandleAsync(AddWordRelationCommand command, CancellationToken cancellationToken = default(CancellationToken))
        {
            var word = await _database.Word.SingleOrDefaultAsync(w => w.Id == command.SourceWordId, cancellationToken);
            if (word == null)
            {
                throw new NotFoundException();
            }

            var relatedWord = await _database.Word.SingleOrDefaultAsync(w => w.Id == command.RelatedWordId, cancellationToken);
            if (relatedWord == null)
            {
                throw new NotFoundException();
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

            command.RelationId = relation.Id;
            return await base.HandleAsync(command, cancellationToken);
        }        
    }
}