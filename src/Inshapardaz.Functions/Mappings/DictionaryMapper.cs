using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Dictionaries;
using Inshapardaz.Functions.Views.Dictionaries;

namespace Inshapardaz.Functions.Mappings
{
    public static class DictionaryMapper
    {
        public static DictionaryView Map(this DictionaryModel source)
            => new DictionaryView
            {
                Id = source.Id,
                Name = source.Name,
                IsPublic = source.IsPublic,
                LanguageId = (int)source.Language,
                Language = source.Language,
                WordCount = source.WordCount
            };

        public static DictionaryModel Map(this DictionaryView source)
            => new DictionaryModel
            {
                Id = source.Id,
                Name = source.Name,
                IsPublic = source.IsPublic,
                Language = (Languages)source.LanguageId,
                WordCount = source.WordCount
            };

        public static DictionaryModel Map(this DictionaryEditView source)
            => new DictionaryModel
            {
                Name = source.Name,
                IsPublic = source.IsPublic,
                Language = (Languages)source.LanguageId,
            };
    }
}
