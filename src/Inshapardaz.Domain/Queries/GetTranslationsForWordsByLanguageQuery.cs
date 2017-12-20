using System;
using System.Collections.Generic;
using Inshapardaz.Domain.Database.Entities;
using Paramore.Darker;

namespace Inshapardaz.Domain.Queries
{
    public class GetTranslationsForWordsByLanguageQuery : IQuery<Dictionary<string, Translation>>
    {
        public GetTranslationsForWordsByLanguageQuery(IEnumerable<string> words, Languages language)
        {
            Words = words;
            Language = language;
        }

        public IEnumerable<string> Words { get; }

        public Languages Language { get; }

        public bool? IsTranspiling { get; set; }
    }
}