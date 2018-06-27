using System;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Repositories.Dictionary;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Dictionary
{
    public class AddDictionaryRequest : RequestBase
    {

        public AddDictionaryRequest(Guid userId, Entities.Dictionary.Dictionary dictionary)
        {
            if (dictionary ==  null)
            {
                throw new ArgumentNullException(nameof(dictionary));
            }

            Dictionary = dictionary;
            Dictionary.UserId = userId;
        }
        public Entities.Dictionary.Dictionary Dictionary { get; }

        public Entities.Dictionary.Dictionary Result { get; set; }
    }

    public class AddDictionaryRequestHandler : RequestHandlerAsync<AddDictionaryRequest>
    {
        private readonly IDictionaryRepository _dictionaryRepository;

        public AddDictionaryRequestHandler(IDictionaryRepository dictionaryRepository)
        {
            _dictionaryRepository = dictionaryRepository;
        }
        public override async Task<AddDictionaryRequest> HandleAsync(AddDictionaryRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            var newDictionry = await _dictionaryRepository.AddDictionary(command.Dictionary, cancellationToken);

            command.Result = newDictionry;

            return await base.HandleAsync(command, cancellationToken);
        }
    } 
}
