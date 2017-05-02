using Darker;

namespace Inshapardaz.Domain.Queries
{
    public class GetDictionaryByTranslationIdQuery : IQuery<Model.Dictionary>
    {
        public int TranslationId { get; set; }
    }
}