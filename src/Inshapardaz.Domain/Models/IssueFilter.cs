namespace Inshapardaz.Domain.Models;

public class IssueFilter
{
    public int? Year { get; set; }

    public int? VolumeNumber { get; set; }

    public StatusType Status { get; set; }

    public AssignmentStatus AssignmentStatus { get; set; }
    public int? TagId { get; set; }
}
