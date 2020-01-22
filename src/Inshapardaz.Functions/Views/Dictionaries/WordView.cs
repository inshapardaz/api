using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Dictionaries;

namespace Inshapardaz.Functions.Views.Dictionaries
{
    public class WordView : ViewWithLinks
    {
        public long Id { get; internal set; }
        public string Title { get; internal set; }
        public string TitleWithMovements { get; internal set; }
        public string Description { get; internal set; }
        public long AttributeValue { get; internal set; }
        public int LanguageId { get; internal set; }
        public string Pronunciation { get; internal set; }
        public GrammaticalType Attributes { get; internal set; }
        public Languages Language { get; internal set; }
    }
}
