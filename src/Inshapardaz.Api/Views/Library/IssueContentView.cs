namespace Inshapardaz.Api.Views.Library;

public class IssueContentView : ViewWithLinks
{
    public long Id { get; set; }

    public int PeriodicalId { get; set; }

    public string MimeType { get; set; }

    public string Language { get; set; }
    public int IssueNumber { get; set; }
    public int VolumeNumber { get; set; }
}
