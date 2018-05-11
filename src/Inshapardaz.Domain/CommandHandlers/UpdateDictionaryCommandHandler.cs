using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Commands;
using Inshapardaz.Domain.Repositories;
using Paramore.Brighter;

namespace Inshapardaz.Domain.CommandHandlers
{
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