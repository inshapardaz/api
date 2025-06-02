namespace Inshapardaz.Domain.Models;

public class BookFilter
{
    public int? AuthorId { get; set; }

    public int? SeriesId { get; set; }

    public int? CategoryId { get; set; }

    public bool? Favorite { get; set; }

    public bool? Read { get; set; }

    public StatusType Status { get; set; }

    public AssignmentStatus AssignmentStatus { get; set; }
    public int? BookShelfId { get; set; }
    public int? TagId { get; set; }
}
