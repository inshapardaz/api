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
            var relation = _queryProcessor.Execute(new RelationshipByIdQuery { Id = command.Relation.Id });
            if (relation == null)
            {
                throw new RecordNotFoundException();
            }

            relation.RelatedWordId = command.Relation.RelatedWordId;
            relation.SourceWordId = command.Relation.SourceWordId;
            relation.RelationType = command.Relation.RelationType;

            _database.SaveChanges();

            return base.Handle(command);
        }
    }
}