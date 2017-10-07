using Darker;
using Inshapardaz.Domain.Database.Entities;

namespace Inshapardaz.Domain.Queries
{
    public class DictionaryByTranslationIdQuery : IQuery<Dictionary>
    {
        public long TranslationId { get; set; }
    }
}