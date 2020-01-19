using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.IndexingService;
using Inshapardaz.Domain.Repositories.Dictionaries;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Dictionaries
{
    public class CreateDictionaryIndexRequest : RequestBase
    {
    }


    public class CreateDictionaryIndexRequestHandler : RequestHandlerAsync<CreateDictionaryIndexRequest>
    {
        private readonly IWriteDictionaryIndex _indexWriter;
        private readonly IDictionaryRepository _dictionaryRepository;
        private readonly IWordRepository _wordRepository;

        public CreateDictionaryIndexRequestHandler(IWriteDictionaryIndex indexWriter, IDictionaryRepository dictionaryRepository, IWordRepository wordRepository)
        {
            _indexWriter = indexWriter;
            _dictionaryRepository = dictionaryRepository;
            _wordRepository = wordRepository;
        }

        public override async Task<CreateDictionaryIndexRequest> HandleAsync(CreateDictionaryIndexRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            var dictionaries = await _dictionaryRepository.GetAllDictionaries(cancellationToken);

            foreach (var dictionary in dictionaries)
            {
                var words = await _wordRepository.GetWords(dictionary.Id, 1, int.MaxValue, cancellationToken);
                _indexWriter.CreateIndex(dictionary.Id, words.Data);

            }

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
