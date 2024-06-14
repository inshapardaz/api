using System;

namespace Inshapardaz.Domain.Models.Library;

public class BookPageModel
{
    public string Text { get; set; }
    public int SequenceNumber { get; set; }
    public int BookId { get; set; }
    public long? ImageId { get; set; }
    public string ImageUrl { get; set; }
    public EditingStatus Status { get; set; }
    public int? WriterAccountId { get; set; }
    public string WriterAccountName { get; set; }
    public DateTime? WriterAssignTimeStamp { get; set; }
    public int? ReviewerAccountId { get; set; }
    public string ReviewerAccountName { get; set; }
    public DateTime? ReviewerAssignTimeStamp { get; set; }
    public long? ChapterId { get; set; }

    public string ChapterTitle { get; set; }
    public BookPageModel PreviousPage { get; set; }
    public BookPageModel NextPage { get; set; }
    public long? ContentId { get; set; }
}
