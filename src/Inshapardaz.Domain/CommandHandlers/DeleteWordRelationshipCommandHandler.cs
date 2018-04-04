using System;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Commands;
using Paramore.Brighter;

namespace Inshapardaz.Domain.CommandHandlers
{
    public class DeleteWordRelationshipCommandHandler : RequestHandlerAsync<DeleteWordRelationshipCommand>
    {
        public override async Task<DeleteWordRelationshipCommand> HandleAsync(DeleteWordRelationshipCommand command, CancellationToken cancellationToken = default(CancellationToken))
        {
            //var relation = await _database.WordRelation.SingleOrDefaultAsync(
            //    r => r.Id == command.RelationshipId && r.SourceWord.DictionaryId == command.DictionaryId, 
            //    cancellationToken);

            //if (relation == null)
            //{
            //    throw new NotFoundException();
            //}

            //_database.WordRelation.Remove(relation);
            //await _database.SaveChangesAsync(cancellationToken);
            throw new NotImplementedException();

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}