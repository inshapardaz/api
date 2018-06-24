using System;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Repositories;
using Inshapardaz.Domain.Repositories.Dictionary;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Commands.Dictionary
{
    public class AddDictionaryCommand : Command
    {
        public AddDictionaryCommand(Entities.Dictionary.Dictionary dictionary)
        {
            Dictionary = dictionary ?? throw new ArgumentNullException(nameof(dictionary));
        }

        public Entities.Dictionary.Dictionary Dictionary { get; }

        public Entities.Dictionary.Dictionary Result { get; set; }
    }

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
