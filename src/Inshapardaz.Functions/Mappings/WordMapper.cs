using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Dictionaries;
using Inshapardaz.Functions.Views.Dictionaries;

namespace Inshapardaz.Functions.Mappings
{
    public static class WordMapper
    {
        public static WordView Map(this WordModel source)
            => new WordView
            {
                Id = source.Id,
                Title = source.Title,
                TitleWithMovements = source.TitleWithMovements,
                Description = source.Description,
                Attributes = source.Attributes,
                AttributeValue = (long)source.Attributes,
                Language = source.Language,
                LanguageId = (int)source.Language,
                Pronunciation = source.Pronunciation
            };

        public static WordModel Map(this WordView source)
            => new WordModel
            {
                Id = source.Id,
                Title = source.Title,
                TitleWithMovements = source.TitleWithMovements,
                Description = source.Description,
                Attributes = (GrammaticalType)source.AttributeValue,
                Language = (Languages)source.LanguageId,
                Pronunciation = source.Pronunciation
            };
    }
}
