namespace Inshapardaz.Api.Views.Library
{
    public class ArticleContentView : ViewWithLinks
    {
        public int Id { get; set; }

        public long ArticleId { get; set; }

        public string Language { get; set; }

        public string Text { get; set; }
    }
}
