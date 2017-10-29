using System.Collections.Generic;
using Paramore.Darker;
using Inshapardaz.Domain.Database.Entities;

namespace Inshapardaz.Domain.Queries
{
    public class GetTranslationsByLanguageQuery : IQuery<IEnumerable<Translation>>
    {
        public long WordId { get; set; }

        public Languages Language { get; set; }
    }
}