using Darker;

namespace Inshapardaz.Domain.Queries
{
    public class DictionaryByTranslationIdQuery : IQuery<Model.Dictionary>
    {
        public long TranslationId { get; set; }
    }
}