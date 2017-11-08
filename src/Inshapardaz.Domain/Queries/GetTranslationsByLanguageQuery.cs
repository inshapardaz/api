using System.Collections.Generic;
using Paramore.Darker;
using Inshapardaz.Domain.Database.Entities;

namespace Inshapardaz.Domain.Queries
{
    public class GetTranslationsByLanguageQuery : IQuery<IEnumerable<Translation>>
    {
        public GetTranslationsByLanguageQuery(long wordId, Languages language)
        {
            WordId = wordId;
            Language = language;
        }

        public long WordId { get; }

        public Languages Language { get; }
    }
}