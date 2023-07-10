namespace Inshapardaz.Domain.Models.Library
{
    public class IssueFilter
    {
        public int? Year { get; set; }

        public int? VolumeNumber { get; set; }

        public StatusType Status { get; set; }

        public AssignmentStatus AssignmentStatus { get; set; }
    }

}
