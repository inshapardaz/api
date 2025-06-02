using Newtonsoft.Json;

namespace Inshapardaz.Api.Views.Library;

public class IssueView : ViewWithLinks
{
    public int Id { get; set; }

    public int IssueNumber { get; set; }

    public int VolumeNumber { get; set; }

    public DateTime IssueDate { get; set; }
    public int PeriodicalId { get; set; }
    public int ArticleCount { get; set; }
    public int PageCount { get; set; }
    
    public List<IssueContentView> Contents { get; set; }
    public string PeriodicalName { get; internal set; }
    
    public string Status { get; set; }
    
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public IEnumerable<PageSummaryView> PageStatus { get; set; }

    public IEnumerable<TagView> Tags { get; set; }
}
