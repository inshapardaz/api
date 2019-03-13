using System.ComponentModel.DataAnnotations;

namespace Inshapardaz.Ports.Database.Entities.Library
{
    public class ArticleText
    {
        [Key]
        public int Id { get; set; }

        public string Content { get; set; }

        public int ArticleId { get; set; }

        public Article Article { get; set; }
    }
}