using System.ComponentModel.DataAnnotations;

namespace Inshapardaz.Ports.Database.Entities.Library
{
    public class Article
    {
        public int Id { get; set; }

        public int SequenceNumber { get; set; }

        public string SeriesName { get; set; }

        public int? SeriesIndex { get; set; }

        [Required]
        public int AuthorId { get; set; }

        public Author Author { get; set; }

        [Required]
        public int IssueId { get; set; }

        public virtual Issue Issue { get; set; }

        public ArticleText Content { get; set; }

    }
}
