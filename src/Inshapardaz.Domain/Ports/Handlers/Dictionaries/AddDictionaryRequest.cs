using System;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Repositories.Dictionaries;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Dictionaries
{
    public class AddDictionaryRequest : RequestBase
    {

        public AddDictionaryRequest(Guid userId, Models.Dictionaries.DictionaryModel dictionary)
        {
            if (dictionary ==  null)
            {
                throw new ArgumentNullException(nameof(dictionary));
            }

            Dictionary = dictionary;
            Dictionary.UserId = userId;
        }
        public Models.Dictionaries.DictionaryModel Dictionary { get; }

        public Models.Dictionaries.DictionaryModel Result { get; set; }
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
