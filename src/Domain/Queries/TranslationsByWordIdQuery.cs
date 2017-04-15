using System.Collections.Generic;
using Darker;
using Inshapardaz.Domain.Model;

namespace Inshapardaz.Domain.Queries
{
    public class  TranslationsByWordIdQuery : IQueryRequest<TranslationsByWordIdQuery.Response>
    {
        public int WordId { get; set; }

        public class Response : IQueryResponse
        {
            public IEnumerable<Translation> Translations { get; set; }
        }
    }
}