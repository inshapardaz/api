using System.Collections.Generic;
using Darker;
using Inshapardaz.Domain.Model;

namespace Inshapardaz.Domain.Queries
{
    public class TranslationsByLanguageQuery : IQuery<IEnumerable<Translation>>
    {
        public int WordId { get; set; }

        public Languages Language { get; set; }
    }
}