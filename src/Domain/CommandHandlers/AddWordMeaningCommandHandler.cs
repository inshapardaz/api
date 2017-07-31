using Inshapardaz.Domain.Commands;
using paramore.brighter.commandprocessor;
using System.Linq;
using Inshapardaz.Domain.Exception;

namespace Inshapardaz.Domain.CommandHandlers
{
    public class AddWordMeaningCommandHandler : RequestHandler<AddWordMeaningCommand>
    {
        private readonly IDatabaseContext _database;

        public AddWordMeaningCommandHandler(IDatabaseContext database)
        {
            _database = database;
        }

        public override AddWordMeaningCommand Handle(AddWordMeaningCommand command)
        {
            var detail = _database.WordDetail.SingleOrDefault(w => w.Id == command.WordDetailId);
            if (detail == null)
            {
                throw new RecordNotFoundException();
            }

            detail.Meaning.Add(command.Meaning);

            _database.SaveChanges();
            return base.Handle(command);
        }
    }
}