using Inshapardaz.Api.Tests.Framework.DataHelpers;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Domain.Helpers.Spelling;

namespace Inshapardaz.Api.Tests.Framework.DataBuilders
{
    public class DictionaryDataBuilder(IDictionaryTestRepository dictionaryRepository)
    {
        private readonly List<long> _dictionaryIds = new();
        private int? _ownerId;
        private string _name;
        private string[] _headwords = [];

        public DictionaryDataBuilder OwnedBy(int? accountId)
        {
            _ownerId = accountId;
            return this;
        }

        public DictionaryDataBuilder WithName(string name)
        {
            _name = name;
            return this;
        }

        // One article per headword, each with a single "meaning of <headword>" definition.
        public DictionaryDataBuilder WithHeadwords(params string[] headwords)
        {
            _headwords = headwords;
            return this;
        }

        public DictionaryDto Build()
        {
            var dictionary = new DictionaryDto
            {
                Name = _name ?? $"Dictionary {RandomData.Name}",
                Description = RandomData.Text,
                Format = "Manual",
                SourceLanguage = "ur",
                TargetLanguage = "en",
                EntryCount = _headwords.Length,
                OwnerId = _ownerId,
                DateCreated = DateTime.UtcNow
            };
            dictionaryRepository.AddDictionary(dictionary);
            _dictionaryIds.Add(dictionary.Id);

            foreach (var headword in _headwords)
            {
                dictionaryRepository.AddArticle(new DictionaryArticleDto
                {
                    DictionaryId = dictionary.Id,
                    Headword = headword,
                    HeadwordNormalised = WordNormaliser.Normalise(headword),
                    Senses = $"[{{\"partOfSpeech\":\"noun\",\"meanings\":[{{\"definition\":\"meaning of {headword}\",\"examples\":[]}}]}}]",
                    RawDefinition = $"meaning of {headword}"
                });
            }

            return dictionary;
        }

        // Dictionaries created through the API in a test, so Cleanup removes them too.
        public void Track(long dictionaryId) => _dictionaryIds.Add(dictionaryId);

        public void Cleanup()
        {
            foreach (var id in _dictionaryIds)
            {
                dictionaryRepository.DeleteDictionary(id);
            }
        }
    }
}
