using System.Collections.Generic;
using Darker;
using Inshapardaz.Domain.Model;

namespace Inshapardaz.Domain.Queries
{
    public class TranslationsByLanguageQuery : IQuery<TranslationsByLanguageQuery.Response>
    {
        public int WordId { get; set; }

        public Languages Language { get; set; }

        public class Response
        {
            public IEnumerable<Translation> Translations { get; set; }
        }
    }
}