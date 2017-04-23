using Darker;
using Inshapardaz.Domain.Commands;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Model;
using Inshapardaz.Domain.Queries;
using paramore.brighter.commandprocessor;

namespace Inshapardaz.Domain.CommandHandlers
{
    public class AddWordRelationCommandHandler : RequestHandler<AddWordRelationCommand>
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

        public override AddWordRelationCommand Handle(AddWordRelationCommand command)
        {
            var word = _queryProcessor.Execute(new WordByIdQuery { Id = command.SourceWordId });
            if (word == null)
            {
                throw new RecordNotFoundException();
            }

            var relatedWord = _queryProcessor.Execute(new WordByIdQuery { Id = command.RelatedWordId });
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

            _database.WordRelations.Add(relation);
            _database.SaveChanges();

            return base.Handle(command);
        }
    }
}