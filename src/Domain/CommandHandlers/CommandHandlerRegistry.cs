using Inshapardaz.Domain.Commands;
using paramore.brighter.commandprocessor;

namespace Inshapardaz.Domain.CommandHandlers
{
    public class CommandHandlerRegistry : SubscriberRegistry
    {
        public CommandHandlerRegistry()
        {
            Register<AddDictionaryCommand, AddDictionaryCommandHandler>();
            Register<AddWordCommand, AddWordCommandHandler>();
            Register<AddWordDetailCommand, AddWordDetailCommandHandler>();
            Register<AddWordRelationCommand, AddWordRelationCommandHandler>();
            Register<AddWordTranslationCommand, AddWordTranslationCommandHandler>();
            Register<AddWordMeaningCommand, AddWordMeaningCommandHandler>();

            Register<UpdateDictionaryCommand, UpdateDictionaryCommandHandler>();
            Register<UpdateWordCommand, UpdateWordCommandHandler>();
            Register<UpdateWordDetailCommand, UpdateWordDetailCommandHandler>();
            Register<UpdateWordRelationCommand, UpdateWordRelationCommandHandler>();
            Register<UpdateWordTranslationCommand, UpdateWordTranslationCommandHandler>();
            Register<UpdateWordMeaningCommand, UpdateWordMeaningCommandHandler>();

            Register<DeleteDictionaryCommand, DeleteDictionaryCommandHandler>();
            Register<DeleteWordCommand, DeleteWordCommandHandler>();
            Register<DeleteWordDetailCommand, DeleteWordDetailCommandHandler>();
            Register<DeleteWordRelationCommand, DeleteWordRelationCommandHandler>();
            Register<DeleteWordTranslationCommand, DeleteWordTranslationCommandHandler>();
            Register<DeleteWordMeaningCommand, DeleteWordMeaningCommandHandler>();
        }
    }
}
