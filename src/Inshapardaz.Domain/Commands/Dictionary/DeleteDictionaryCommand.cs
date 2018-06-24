using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Repositories;
using Inshapardaz.Domain.Repositories.Dictionary;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Commands.Dictionary
{
    public class DeleteDictionaryCommand : Command
    {
        public DeleteDictionaryCommand(int dictionaryId)
        {
            DictionaryId = dictionaryId;
        }

        public int DictionaryId { get; }
    }

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
