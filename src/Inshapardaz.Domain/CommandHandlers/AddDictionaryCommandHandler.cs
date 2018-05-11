using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Commands;
using Inshapardaz.Domain.Repositories;
using Paramore.Brighter;

namespace Inshapardaz.Domain.CommandHandlers
{
    public class AddDictionaryCommandHandler : RequestHandlerAsync<AddDictionaryCommand>
    {
        private readonly IDictionaryRepository _dictionaryRepository;

        public AddDictionaryCommandHandler(IDictionaryRepository dictionaryRepository)
        {
            _dictionaryRepository = dictionaryRepository;
        }

        public override async Task<AddDictionaryCommand> HandleAsync(AddDictionaryCommand command, CancellationToken cancellationToken = default(CancellationToken))
        {
            command.Result = await _dictionaryRepository.AddDictionary(command.Dictionary, cancellationToken);

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}