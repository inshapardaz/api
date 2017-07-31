using System.Linq;
using Inshapardaz.Domain.Commands;
using Inshapardaz.Domain.Exception;
using paramore.brighter.commandprocessor;

namespace Inshapardaz.Domain.CommandHandlers
{
    public class DeleteDictionaryCommandHandler : RequestHandler<DeleteDictionaryCommand>
    {
        private readonly IDatabaseContext _database;

        public DeleteDictionaryCommandHandler(IDatabaseContext database)
        {
            _database = database;
        }

        public override DeleteDictionaryCommand Handle(DeleteDictionaryCommand command)
        {
            var d = _database.Dictionary.SingleOrDefault(
                x => x.Id == command.DictionaryId && x.UserId == command.UserId);

            if (d == null || d.Id != command.DictionaryId)
            {
                throw new RecordNotFoundException();
            }

            _database.Dictionary.Remove(d);

            _database.SaveChanges();

            return base.Handle(command);
        }
    }
}