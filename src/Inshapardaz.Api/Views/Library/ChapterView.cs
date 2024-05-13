namespace Inshapardaz.Api.Views.Library;

public class ChapterView : ViewWithLinks
{
    public long Id { get; set; }

    public int ChapterNumber { get; set; }

    public string Title { get; set; }

    public int BookId { get; set; }
    public string Status { get; set; }

    public int? WriterAccountId { get; set; }
    public string WriterAccountName { get; set; }
    public DateTime? WriterAssignTimeStamp { get; set; }
    public int? ReviewerAccountId { get; set; }
    public string ReviewerAccountName { get; set; }
    public DateTime? ReviewerAssignTimeStamp { get; set; }

    public IEnumerable<ChapterContentView> Contents { get; set; }
}
