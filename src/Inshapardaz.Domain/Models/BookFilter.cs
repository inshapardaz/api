namespace Inshapardaz.Domain.Models.Library
{
    public class BookFilter
    {
        public int? AuthorId { get; set; }

        public int? SeriesId { get; set; }

        public int? CategoryId { get; set; }

        public bool? Favorite { get; set; }

        public bool? Read { get; set; }

        public BookStatuses Status { get; set; }

        public BookAssignmentStatus AssignmentStatus { get; set; }
        public int? BookShelfId { get; set; }
    }

    public enum BookAssignmentStatus
    { 
        None = 0,
        Writer =1,
        Reviewer = 2
    }

    public class IssueFilter
    {
        public int? Year { get; set; }

        public int? VolumeNumber { get; set; }

        public BookStatuses Status { get; set; }

        public BookAssignmentStatus AssignmentStatus { get; set; }
    }

}
