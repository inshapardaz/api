using Newtonsoft.Json;

namespace Inshapardaz.Api.Views.Library;

public class IssuePageView : ViewWithLinks
{

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string Text { get; set; }
    public int PeriodicalId { get; set; }
    public int VolumeNumber { get; set; }
    public int IssueNumber { get; set; }
    public int SequenceNumber { get; set; }
    public string Status { get; set; }
    public int? WriterAccountId { get; set; }
    public string WriterAccountName { get; set; }
    public DateTime? WriterAssignTimeStamp { get; set; }
    public int? ReviewerAccountId { get; set; }
    public string ReviewerAccountName { get; set; }
    public DateTime? ReviewerAssignTimeStamp { get; set; }
    public string ArticleName { get; internal set; }
    public long? ArticleId { get; internal set; }
}
