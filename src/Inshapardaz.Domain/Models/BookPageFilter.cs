namespace Inshapardaz.Domain.Models;

public class PageFilter
{
    public EditingStatus? Status { get; set; }

    public AssignmentFilter? WrtiterAssignmentFilter { get; set; }

    public int? AccountId { get; set; }

    public AssignmentFilter? ReviewerAssignmentFilter { get; set; }
}
