using System;
using System.Collections.Generic;
using System.Text;
using Inshapardaz.Api.View;

namespace Inshapardaz.Api.IntegrationTests.Helpers
{
    public static class ModelExtentions
    {
        public static WordView Map(this Domain.Entities.Word word)
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
    }
}
