using Inshapardaz.Domain.Models;

namespace Inshapardaz.Functions.Views.Dictionaries
{
    public class DictionaryView : ViewWithLinks
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsPublic { get; set; }
        public Languages Language { get; set; }
        public int LanguageId { get; set; }
        public int WordCount { get; set; }
    }

    public class DictionaryEditView
    {
        public string Name { get; set; }
        public bool IsPublic { get; set; }
        public int LanguageId { get; set; }
    }
}
