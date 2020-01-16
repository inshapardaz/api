using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Repositories.Dictionaries;
using Paramore.Darker;

namespace Inshapardaz.Domain.Ports.Dictionaries
{
    public class GetDictionariesQuery : IQuery<IEnumerable<Domain.Entities.Dictionaries.Dictionary>>
    {
        public Guid UserId { get; set; }
    }

    public class GetDictionariesQueryHandler : QueryHandlerAsync<GetDictionariesQuery, IEnumerable<Domain.Entities.Dictionaries.Dictionary>>
    {
        private readonly IDictionaryRepository _dictionaryRepository;
        private readonly IWordRepository _wordRepository;

        public GetDictionariesQueryHandler(IDictionaryRepository dictionaryRepository, IWordRepository wordRepository)
        {
            _dictionaryRepository = dictionaryRepository;
            _wordRepository = wordRepository;
        }

        public override async Task<IEnumerable<Domain.Entities.Dictionaries.Dictionary>> ExecuteAsync(GetDictionariesQuery query, CancellationToken cancellationToken = default(CancellationToken))
        {
            var dictionaries = query.UserId == Guid.Empty 
                ? await _dictionaryRepository.GetPublicDictionaries(cancellationToken)
                : await _dictionaryRepository.GetAllDictionariesForUser(query.UserId, cancellationToken);

            foreach (var dictionary in dictionaries)
            {
                var wordCount = await _wordRepository.GetWordCountByDictionary(dictionary.Id, cancellationToken);
                dictionary.WordCount = wordCount;

                var downloads = await _dictionaryRepository.GetDictionaryDownloads(dictionary.Id, cancellationToken);
                dictionary.Downloads = downloads;
            }

            return dictionaries;
        }
    }
}
