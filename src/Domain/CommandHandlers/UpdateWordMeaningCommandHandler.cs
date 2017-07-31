using System.Linq;
using Inshapardaz.Domain.Commands;
using Inshapardaz.Domain.Exception;
using paramore.brighter.commandprocessor;

namespace Inshapardaz.Domain.CommandHandlers
{
    public class UpdateWordMeaningCommandHandler : RequestHandler<UpdateWordMeaningCommand>
    {
        private readonly IDatabaseContext _database;

        public UpdateWordMeaningCommandHandler(IDatabaseContext database)
        {
            _database = database;
        }

        public override UpdateWordMeaningCommand Handle(UpdateWordMeaningCommand command)
        {
            var meaning = _database.Meaning.SingleOrDefault(x => x.Id == command.Meaning.Id);

            if (meaning == null)
            {
                throw new RecordNotFoundException();
            }

            meaning.Context = command.Meaning.Context;
            meaning.Value = command.Meaning.Value;
            meaning.Example = command.Meaning.Example;

            _database.SaveChanges();

            return base.Handle(command);
        }
    }
}