using Newtonsoft.Json;

namespace Inshapardaz.Api.Views.Library;

public class BookPageView : ViewWithLinks
{
    public int SequenceNumber { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string Text { get; set; }

    public int BookId { get; set; }
    public string Status { get; set; }
    public int? WriterAccountId { get; set; }
    public string WriterAccountName { get; set; }
    public DateTime? WriterAssignTimeStamp { get; set; }
    public int? ReviewerAccountId { get; set; }
    public string ReviewerAccountName { get; set; }
    public DateTime? ReviewerAssignTimeStamp { get; set; }
    public long? ChapterId { get; set; }
    public string ChapterTitle { get; set; }
}
