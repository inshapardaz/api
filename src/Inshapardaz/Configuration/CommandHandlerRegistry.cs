using Inshapardaz.Api.Adapter;
using Inshapardaz.Domain.CommandHandlers;
using Inshapardaz.Domain.Commands;
using paramore.brighter.commandprocessor;

namespace Inshapardaz.Api.Configuration
{
    public class CommandHandlerRegistry : SubscriberRegistry
    {
        public CommandHandlerRegistry()
        {
            // Adapters
            Register<GetEntryCommand, GetEntryCommandHandler>();

            // Domain
            RegisterAsync<AddDictionaryCommand, AddDictionaryCommandHandler>();
            RegisterAsync<AddDictionaryDownloadCommand, AddDictionaryDownloadCommandHandler>();
            RegisterAsync<AddWordCommand, AddWordCommandHandler>();
            RegisterAsync<AddWordDetailCommand, AddWordDetailCommandHandler>();
            RegisterAsync<AddWordRelationCommand, AddWordRelationCommandHandler>();
            RegisterAsync<AddWordTranslationCommand, AddWordTranslationCommandHandler>();
            RegisterAsync<AddWordMeaningCommand, AddWordMeaningCommandHandler>();

            RegisterAsync<UpdateDictionaryCommand, UpdateDictionaryCommandHandler>();
            RegisterAsync<UpdateWordCommand, UpdateWordCommandHandler>();
            RegisterAsync<UpdateWordDetailCommand, UpdateWordDetailCommandHandler>();
            RegisterAsync<UpdateWordRelationCommand, UpdateWordRelationCommandHandler>();
            RegisterAsync<UpdateWordTranslationCommand, UpdateWordTranslationCommandHandler>();
            RegisterAsync<UpdateWordMeaningCommand, UpdateWordMeaningCommandHandler>();

            RegisterAsync<DeleteDictionaryCommand, DeleteDictionaryCommandHandler>();
            RegisterAsync<DeleteWordCommand, DeleteWordCommandHandler>();
            RegisterAsync<DeleteWordDetailCommand, DeleteWordDetailCommandHandler>();
            RegisterAsync<DeleteWordRelationCommand, DeleteWordRelationCommandHandler>();
            RegisterAsync<DeleteWordTranslationCommand, DeleteWordTranslationCommandHandler>();
            RegisterAsync<DeleteWordMeaningCommand, DeleteWordMeaningCommandHandler>();
        }
    }
}
