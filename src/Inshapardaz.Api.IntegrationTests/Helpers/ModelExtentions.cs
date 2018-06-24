using System;
using System.Collections.Generic;
using System.Text;
using Inshapardaz.Api.View;
using Inshapardaz.Api.View.Dictionary;
using Inshapardaz.Domain.Entities.Dictionary;

namespace Inshapardaz.Api.IntegrationTests.Helpers
{
    public static class ModelExtentions
    {
        public static WordView Map(this Word word)
        {
            return new WordView
            {
                Id = word.Id,
                Title = word.Title,
                TitleWithMovements = word.TitleWithMovements,
                Language = word.Language.ToString(),
                LanguageId = (int)word.Language,
                Attributes = word.Attributes.ToString(),
                AttributeValue = (int)word.Attributes,
                Pronunciation = word.Pronunciation,
                Description = word.Description
            };
        }

        public static MeaningView Map(this Meaning meaning)
        {
            return new MeaningView
            {
                Id = meaning.Id,
                Context = meaning.Context,
                Value = meaning.Value,
                Example = meaning.Example,
                WordId = meaning.WordId
            };
        }

        public static TranslationView Map(this Translation translation)
        {
            return new TranslationView
            {
                Id = translation.Id,
                Language = translation.Language.ToString(),
                LanguageId = (int)translation.Language,
                Value = translation.Value,
                IsTranspiling = translation.IsTrasnpiling,
                WordId = translation.WordId
            };
        }

        public static RelationshipView Map(this WordRelation relation)
        {
            return new RelationshipView
            {
                Id = relation.Id,
                SourceWordId = relation.SourceWordId,
                RelatedWordId = relation.RelatedWordId,
                RelationTypeId = (int)relation.RelationType
            };
        }
    }
}
