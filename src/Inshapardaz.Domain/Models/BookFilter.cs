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
    }
}
