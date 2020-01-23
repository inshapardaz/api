using System;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Models.Dictionaries;
using Inshapardaz.Domain.Repositories.Dictionaries;
using Paramore.Brighter;
using Paramore.Darker;

namespace Inshapardaz.Domain.Ports.Dictionaries
{
    public class GetDictionaryByIdQuery : IQuery<DictionaryModel>
    {
        public GetDictionaryByIdQuery(int dictionaryId, Guid userId)
        {
            DictionaryId = dictionaryId;
            UserId = userId;
        }

        public int DictionaryId { get; private set; }
        public Guid UserId { get; set; }
    }

    public class GetDictionaryByIdQueryHandler : QueryHandlerAsync<GetDictionaryByIdQuery, DictionaryModel>
    {
        private readonly IDictionaryRepository _dictionaryRepository;
        private readonly IWordRepository _wordRepository;

        public GetDictionaryByIdQueryHandler(IDictionaryRepository dictionaryRepository, IWordRepository wordRepository)
        {
            _dictionaryRepository = dictionaryRepository;
            _wordRepository = wordRepository;
        }

        [DictionaryRequestValidation(1, HandlerTiming.Before)]
        public override async Task<DictionaryModel> ExecuteAsync(GetDictionaryByIdQuery query, CancellationToken cancellationToken = new CancellationToken())
        {
            var dictionary = await _dictionaryRepository.GetDictionaryById(query.DictionaryId, cancellationToken);

            if (dictionary != null)
            {
                if (!dictionary.IsPublic && dictionary.UserId != query.UserId)
                {
                    throw new UnauthorizedException();
                }

                dictionary.WordCount = await _wordRepository.GetWordCountByDictionary(query.DictionaryId, cancellationToken);
                dictionary.Downloads = await _dictionaryRepository.GetDictionaryDownloads(query.DictionaryId, cancellationToken);
            }

            return dictionary;
        }
    }
}
