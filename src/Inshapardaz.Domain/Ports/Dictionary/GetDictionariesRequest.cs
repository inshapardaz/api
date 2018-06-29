using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Repositories.Dictionary;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Dictionary
{
    public class GetDictionariesRequest : RequestBase
    {
        public GetDictionariesRequest()
            :this(Guid.Empty)
        {
        }

        public GetDictionariesRequest(Guid userId)
        {
            UserId = userId;
        }

        public Guid UserId { get; }

        public IEnumerable<Domain.Entities.Dictionary.Dictionary> Result { get; set; }
    }

    public class GetDictionariesRequestHandler : RequestHandlerAsync<GetDictionariesRequest>
    {
        private readonly IDictionaryRepository _dictionaryRepository;
        private readonly IWordRepository _wordRepository;

        public GetDictionariesRequestHandler(IDictionaryRepository dictionaryRepository, IWordRepository wordRepository)
        {
            _dictionaryRepository = dictionaryRepository;
            _wordRepository = wordRepository;
        }

        public override async Task<GetDictionariesRequest> HandleAsync(GetDictionariesRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            var dictionaries = command.UserId == Guid.Empty 
                ? await _dictionaryRepository.GetPublicDictionaries(cancellationToken)
                : await _dictionaryRepository.GetAllDictionariesForUser(command.UserId, cancellationToken);

            foreach (var dictionary in dictionaries)
            {
                var wordCount = await _wordRepository.GetWordCountByDictionary(dictionary.Id, cancellationToken);
                dictionary.WordCount = wordCount;

                var downloads = await _dictionaryRepository.GetDictionaryDownloads(dictionary.Id, cancellationToken);
                dictionary.Downloads = downloads;
            }

            command.Result = dictionaries;

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
