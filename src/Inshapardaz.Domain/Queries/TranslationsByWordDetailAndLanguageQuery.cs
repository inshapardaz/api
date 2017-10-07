using System.Collections.Generic;
using Paramore.Darker;
using Inshapardaz.Domain.Database.Entities;

namespace Inshapardaz.Domain.Queries
{
    public class TranslationsByWordDetailAndLanguageQuery : IQuery<IEnumerable<Translation>>
    {
        public long WordDetailId { get; set; }

        public Languages Language { get; set; }
    }
}