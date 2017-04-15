using Darker;
using Inshapardaz.Domain.Commands;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Queries;
using paramore.brighter.commandprocessor;

namespace Inshapardaz.Domain.CommandHandlers
{
    public class UpdateWordRelationCommandHandler : RequestHandler<UpdateWordRelationCommand>
    {
        private readonly IDatabaseContext _database;
        private readonly IQueryProcessor _queryProcessor;

        public UpdateWordRelationCommandHandler(
            IDatabaseContext database,
            IQueryProcessor queryProcessor)
        {
            _database = database;
            _queryProcessor = queryProcessor;
        }

        public override UpdateWordRelationCommand Handle(UpdateWordRelationCommand command)
        {
            var response = _queryProcessor.Execute(new RelationshipByIdQuery { Id = command.Relation.Id });
            if (response.Relation == null)
            {
                throw new RecordNotFoundException();
            }

            response.Relation.RelatedWordId = command.Relation.RelatedWordId;
            response.Relation.SourceWordId = command.Relation.SourceWordId;
            response.Relation.RelationType = command.Relation.RelationType;

            _database.SaveChanges();

            return base.Handle(command);
        }
    }
}