using System.Collections.Generic;
using Darker;
using Inshapardaz.Domain.Model;

namespace Inshapardaz.Domain.Queries
{
    public class TranslationsByWordIdQuery : IQuery<IEnumerable<Translation>>
    {
        public long WordId { get; set; }
    }
}