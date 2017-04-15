using Inshapardaz.Domain.Commands;
using Inshapardaz.Domain.Exception;
using paramore.brighter.commandprocessor;
using System.Linq;

namespace Inshapardaz.Domain.CommandHandlers
{
    public class DeleteWordMeaningCommandHandler : RequestHandler<DeleteWordMeaningCommand>
    {
        private readonly IDatabaseContext _database;

        public DeleteWordMeaningCommandHandler(IDatabaseContext database)
        {
            _database = database;
        }

        public override DeleteWordMeaningCommand Handle(DeleteWordMeaningCommand command)
        {
            var meaning = _database.Meanings.SingleOrDefault(x => x.Id == command.Meaning.Id);

            if (meaning == null)
            {
                throw new RecordNotFoundException();
            }

            _database.Meanings.Remove(meaning);
            _database.SaveChanges();

            return base.Handle(command);
        }
    }
}