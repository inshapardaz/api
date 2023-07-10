namespace Inshapardaz.Domain.Models.Library
{
    public class ArticleFilter
    {
        public int? AuthorId { get; set; }

        public int? CategoryId { get; set; }

        public bool? Favorite { get; set; }

        public bool? Read { get; set; }

        public StatusType Status { get; set; }

        public AssignmentStatus AssignmentStatus { get; set; }
    }
}
