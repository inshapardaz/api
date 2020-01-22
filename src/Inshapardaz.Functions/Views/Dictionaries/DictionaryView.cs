using Inshapardaz.Domain.Models;

namespace Inshapardaz.Functions.Views.Dictionaries
{
    public class DictionaryView : ViewWithLinks
    {
        public int Id { get; internal set; }
        public string Name { get; internal set; }
        public bool IsPublic { get; internal set; }
        public Languages Language { get; internal set; }

        public int LanguageId { get; internal set; }
        public int WordCount { get; internal set; }
    }
}
