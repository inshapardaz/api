using System.Linq;
using Inshapardaz.Domain.Commands;
using Inshapardaz.Domain.Exception;
using paramore.brighter.commandprocessor;

namespace Inshapardaz.Domain.CommandHandlers
{
    public class DeleteDictionaryCommandHandler : RequestHandler<DeleteDictionaryCommand>
    {
        private readonly IDatabase _database;

        public DeleteDictionaryCommandHandler(IDatabase database)
        {
            _database = database;
        }

        public override DeleteDictionaryCommand Handle(DeleteDictionaryCommand command)
        {
            var w = _database.Dictionaries.SingleOrDefault(x => x.Id == command.DictionaryId);

            if (w == null || w.Id != command.DictionaryId)
            {
                throw new RecordNotFoundException();
            }

            _database.Dictionaries.Remove(w);

            _database.SaveChanges();

            return base.Handle(command);
        }
    }
}
