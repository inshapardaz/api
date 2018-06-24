using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Repositories;
using Inshapardaz.Domain.Repositories.Dictionary;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Commands.Dictionary
{
    public class UpdateDictionaryCommand : Command
    {
        public UpdateDictionaryCommand(Entities.Dictionary.Dictionary dictionary)
        {
            Dictionary = dictionary;
        }

        public Entities.Dictionary.Dictionary Dictionary { get; }
    }

    public class UpdateDictionaryCommandHandler : RequestHandlerAsync<UpdateDictionaryCommand>
    {
        private readonly IDictionaryRepository _dictionaryRepository;

        public UpdateDictionaryCommandHandler(IDictionaryRepository dictionaryRepository)
        {
            _dictionaryRepository = dictionaryRepository;
        }

        public override async Task<UpdateDictionaryCommand> HandleAsync(UpdateDictionaryCommand command, CancellationToken cancellationToken = default(CancellationToken))
        {
            await _dictionaryRepository.UpdateDictionary(command.Dictionary.Id, command.Dictionary, cancellationToken);
            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
