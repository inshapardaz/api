using System.Collections.Generic;
using Darker;
using Inshapardaz.Domain.Model;

namespace Inshapardaz.Domain.Queries
{
    public class TranslationsByLanguageQuery : IQueryRequest<TranslationsByLanguageQuery.Response>
    {
        public int WordId { get; set; }

        public Languages Language { get; set; }

        public class Response : IQueryResponse
        {
            public IEnumerable<Translation> Translations { get; set; }
        }
    }
}