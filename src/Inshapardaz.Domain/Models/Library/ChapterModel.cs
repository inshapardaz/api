using System;
using System.Collections.Generic;

namespace Inshapardaz.Domain.Models.Library;

public class ChapterModel
{
    public long Id { get; set; }

    public int ChapterNumber { get; set; }

    public string Title { get; set; }

    public int BookId { get; set; }

    public EditingStatus Status { get; set; }

    public int? WriterAccountId { get; set; }
    public string WriterAccountName { get; set; }
    public DateTime? WriterAssignTimeStamp { get; set; }
    public int? ReviewerAccountId { get; set; }
    public string ReviewerAccountName { get; set; }
    public DateTime? ReviewerAssignTimeStamp { get; set; }

    public List<ChapterContentModel> Contents { get; set; } = new List<ChapterContentModel>();
    public ChapterModel PreviousChapter { get; internal set; }
    public ChapterModel NextChapter { get; internal set; }
}
