namespace Inshapardaz.Api.Views.Library
{
    public class ArticleContentView : ViewWithLinks
    {
        public int Id { get; set; }

        public int BookId { get; set; }

        public int ChapterNumber { get; set; }

        public string Language { get; set; }

        public string Text { get; set; }
    }
}
