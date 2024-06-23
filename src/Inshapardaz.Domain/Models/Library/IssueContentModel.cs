namespace Inshapardaz.Domain.Models.Library;

public class IssueContentModel
{
    public long Id { get; set; }

    public int PeriodicalId { get; set; }
    public int IssueNumber { get; set; }

    public string ContentUrl { get; set; }

    public string MimeType { get; set; }

    public string Language { get; set; }

    public long FileId { get; set; }
    public int VolumeNumber { get; set; }
}
