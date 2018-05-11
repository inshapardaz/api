namespace Inshapardaz.Ports.Elasticsearch.CommandHandlers
{
    public class UpdateWordRelationCommandHandler : RequestHandlerAsync<UpdateWordRelationCommand>
    {
        //private readonly IDatabaseContext _database;

        //public UpdateWordRelationCommandHandler(IDatabaseContext database)
        //{
        //    _database = database;
        //}

        //public override async Task<UpdateWordRelationCommand> HandleAsync(UpdateWordRelationCommand command, CancellationToken cancellationToken = default(CancellationToken))
        //{
        //    var relation = await _database.WordRelation.SingleOrDefaultAsync(
        //        r => r.Id == command.Relation.Id && r.SourceWord.DictionaryId == command.DictionaryId, 
        //        cancellationToken);
        //    if (relation == null)
        //    {
        //        throw new NotFoundException();
        //    }

        //    relation.RelatedWordId = command.Relation.RelatedWordId;
        //    relation.RelationType = command.Relation.RelationType;

        //    await _database.SaveChangesAsync(cancellationToken);

        //    return await base.HandleAsync(command, cancellationToken);
        //}
    }
}