namespace Inshapardaz.Domain.Models.Library
{
    public class Article
    {
        public int Id { get; set; }

        public int SequenceNumber { get; set; }

        public string SeriesName { get; set; }

        public int? SeriesIndex { get; set; }

        public int AuthorId { get; set; }

        public AuthorModel Author { get; set; }

        public int IssueId { get; set; }
    }
}