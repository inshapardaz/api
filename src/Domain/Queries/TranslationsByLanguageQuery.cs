using System.Collections.Generic;
using Darker;
using Inshapardaz.Domain.Database.Entities;

namespace Inshapardaz.Domain.Queries
{
    public class TranslationsByLanguageQuery : IQuery<IEnumerable<Translation>>
    {
        public long WordId { get; set; }

        public Languages Language { get; set; }
    }
}