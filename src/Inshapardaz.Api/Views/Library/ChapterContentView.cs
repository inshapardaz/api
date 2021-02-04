using Newtonsoft.Json;

namespace Inshapardaz.Api.Views.Library
{
    public class ChapterContentView : ViewWithLinks
    {
        public int Id { get; set; }

        public int BookId { get; set; }

        public int ChapterId { get; set; }

        public int ChapterNumber { get; set; }

        public string Language { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Text { get; internal set; }
    }
}
