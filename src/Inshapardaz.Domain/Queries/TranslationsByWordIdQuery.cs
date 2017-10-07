using System.Collections.Generic;
using Paramore.Darker;
using Inshapardaz.Domain.Database.Entities;

namespace Inshapardaz.Domain.Queries
{
    public class TranslationsByWordIdQuery : IQuery<IEnumerable<Translation>>
    {
        public long WordId { get; set; }
    }
}