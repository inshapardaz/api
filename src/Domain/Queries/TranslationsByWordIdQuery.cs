using System.Collections.Generic;
using Darker;
using Inshapardaz.Domain.Model;

namespace Inshapardaz.Domain.Queries
{
    public class  TranslationsByWordIdQuery : IQuery<TranslationsByWordIdQuery.Response>
    {
        public int WordId { get; set; }

        public class Response
        {
            public IEnumerable<Translation> Translations { get; set; }
        }
    }
}