namespace Inshapardaz.Domain.Models.Library
{
    public class BookPageFilter
    {
        public EditingStatus? Status { get; set; }

        public AssignmentFilter? AssignmentFilter { get; set; }

        public int? AccountId { get; set; }

        public AssignmentFilter? ReviewerAssignmentFilter { get; set; }
    }
}
