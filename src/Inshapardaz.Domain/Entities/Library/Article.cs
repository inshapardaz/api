namespace Inshapardaz.Domain.Entities.Library
{
    public class Article
    {
        public int Id { get; set; }

        public int SequenceNumber { get; set; }

        public string SeriesName { get; set; }

        public int? SeriesIndex { get; set; }

        public int AuthorId { get; set; }

        public Author Author { get; set; }

        public int IssueId { get; set; }
    }
}