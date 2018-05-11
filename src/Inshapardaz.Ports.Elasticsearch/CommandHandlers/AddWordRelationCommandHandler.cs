using System;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Ports.Elasticsearch.CommandHandlers
{
    public class AddWordRelationCommandHandler : RequestHandlerAsync<AddWordRelationCommand>
    {
        //    private readonly IDatabaseContext _database;

        //    public AddWordRelationCommandHandler(IDatabaseContext database)
        //    {
        //        _database = database;
        //    }

        //    public override async Task<AddWordRelationCommand> HandleAsync(AddWordRelationCommand command, CancellationToken cancellationToken = default(CancellationToken))
        //    {
        //        if (command.SourceWordId == command.RelatedWordId)
        //        {
        //            throw new BadRequestException();
        //        }

        //        var sourceWord = await _database.Word.SingleOrDefaultAsync(
        //            w => w.Id == command.SourceWordId && w.DictionaryId == command.DictionaryId, 
        //            cancellationToken);

        //        if (sourceWord == null)
        //        {
        //            throw new NotFoundException();
        //        }

        //        var relatedWord = await _database.Word.SingleOrDefaultAsync(
        //            w => w.Id == command.RelatedWordId && w.DictionaryId == command.DictionaryId, 
        //            cancellationToken);

        //        if (relatedWord == null || sourceWord.DictionaryId != relatedWord.DictionaryId)
        //        {
        //            throw new BadRequestException();
        //        }

        //        var relation = new WordRelation
        //        {
        //            SourceWord = sourceWord,
        //            SourceWordId = command.SourceWordId,
        //            RelatedWord = relatedWord,
        //            RelatedWordId = command.RelatedWordId,
        //            RelationType = command.RelationType
        //        };

        //        _database.WordRelation.Add(relation);
        //        await _database.SaveChangesAsync(cancellationToken);

        //        command.Result = relation.Id;
        //        return await base.HandleAsync(command, cancellationToken);
        //    }        
        //}
        public override Task<AddWordRelationCommand> HandleAsync(AddWordRelationCommand command, CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();

            return base.HandleAsync(command, cancellationToken);
        }
    }
}