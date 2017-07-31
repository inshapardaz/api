using System.Linq;
using Inshapardaz.Domain.Commands;
using Inshapardaz.Domain.Exception;
using paramore.brighter.commandprocessor;

namespace Inshapardaz.Domain.CommandHandlers
{
    public class UpdateDictionaryCommandHandler : RequestHandler<UpdateDictionaryCommand>
    {
        private readonly IDatabaseContext _database;

        public UpdateDictionaryCommandHandler(IDatabaseContext database)
        {
            _database = database;
        }

        public override UpdateDictionaryCommand Handle(UpdateDictionaryCommand command)
        {
            var existingDictionary =
                _database.Dictionary.SingleOrDefault(d => d.UserId == command.Dictionary.UserId && d.Id == command.Dictionary.Id);

            if (existingDictionary == null)
            {
                throw new RecordNotFoundException();
            }

            existingDictionary.Name = command.Dictionary.Name;
            existingDictionary.Language = command.Dictionary.Language;
            existingDictionary.IsPublic = command.Dictionary.IsPublic;

            _database.SaveChanges();

            return base.Handle(command);
        }
    }
}