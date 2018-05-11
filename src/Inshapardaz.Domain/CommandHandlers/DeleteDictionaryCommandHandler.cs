using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Commands;
using Inshapardaz.Domain.Repositories;
using Paramore.Brighter;

namespace Inshapardaz.Domain.CommandHandlers
{
    public class DeleteDictionaryCommandHandler : RequestHandlerAsync<DeleteDictionaryCommand>
    {
        private readonly IDictionaryRepository _dictionaryRepository;

        public DeleteDictionaryCommandHandler(IDictionaryRepository dictionaryRepository)
        {
            _dictionaryRepository = dictionaryRepository;
        }

        public override async Task<DeleteDictionaryCommand> HandleAsync(DeleteDictionaryCommand command, CancellationToken cancellationToken = default(CancellationToken))
        {
            await _dictionaryRepository.DeleteDictionary(command.DictionaryId, cancellationToken);
            return await base.HandleAsync(command, cancellationToken);
        }
    }
}