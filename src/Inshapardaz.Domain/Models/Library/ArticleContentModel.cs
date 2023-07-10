namespace Inshapardaz.Domain.Models.Library
{
    public class ArticleContentModel
    {
        public int Id { get; set; }

        public int BookId { get; set; }

        public int ChapterNumber { get; set; }

        public string Text { get; set; }
        public string Language { get; set; }
    }
}
