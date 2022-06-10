namespace Inshapardaz.Domain.Models.Library
{
    public class BookPageFilter
    {
        public PageStatuses? Status { get; set; }

        public AssignmentFilter? AssignmentFilter { get; set; }

        public int? AccountId { get; set; }

        public AssignmentFilter? ReviewerAssignmentFilter { get; set; }
    }
}
