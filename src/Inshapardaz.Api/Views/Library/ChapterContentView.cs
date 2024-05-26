using Newtonsoft.Json;

namespace Inshapardaz.Api.Views.Library;

public class ChapterContentView : ViewWithLinks
{
    public long Id { get; set; }

    public int BookId { get; set; }

    public long ChapterId { get; set; }

    public int ChapterNumber { get; set; }

    public string Language { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string Text { get; internal set; }
}
