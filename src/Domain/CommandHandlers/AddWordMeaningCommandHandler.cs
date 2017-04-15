using Inshapardaz.Domain.Commands;
using paramore.brighter.commandprocessor;
using System.Linq;
using Inshapardaz.Domain.Exception;

namespace Inshapardaz.Domain.CommandHandlers
{
    public class AddWordMeaningCommandHandler : RequestHandler<AddWordMeaningCommand>
    {
        private readonly IDatabase _database;

        public AddWordMeaningCommandHandler(IDatabase database)
        {
            _database = database;
        }

        public override AddWordMeaningCommand Handle(AddWordMeaningCommand command)
        {
            var detail = _database.WordDetails.SingleOrDefault(w => w.Id == command.WordDetailId);
            if (detail == null)
            {
                throw new RecordNotFoundException();
            }

            detail.Meanings.Add(command.Meaning);

            _database.SaveChanges();
            return base.Handle(command);
        }
    }
}
