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
            if (command.SourceWordId == command.RelatedWordId)
            {
                throw new BadRequestException();
            }

            var sourceWord = await _database.Word.SingleOrDefaultAsync(
                w => w.Id == command.SourceWordId && w.DictionaryId == command.DictionaryId, 
                cancellationToken);

            if (sourceWord == null)
            {
                throw new NotFoundException();
            }

            var relatedWord = await _database.Word.SingleOrDefaultAsync(
                w => w.Id == command.RelatedWordId && w.DictionaryId == command.DictionaryId, 
                cancellationToken);

            if (relatedWord == null || sourceWord.DictionaryId != relatedWord.DictionaryId)
            {
                throw new BadRequestException();
            }

            var relation = new WordRelation
            {
                SourceWord = sourceWord,
                SourceWordId = command.SourceWordId,
                RelatedWord = relatedWord,
                RelatedWordId = command.RelatedWordId,
                RelationType = command.RelationType
            };

            _database.WordRelation.Add(relation);
            await _database.SaveChangesAsync(cancellationToken);

            command.Result = relation.Id;
            return await base.HandleAsync(command, cancellationToken);
        }        
    }
}