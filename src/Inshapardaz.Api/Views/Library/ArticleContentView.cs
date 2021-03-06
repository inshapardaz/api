namespace Inshapardaz.Api.Views.Library
{
    public class ArticleContentView : ViewWithLinks
    {
        public int Id { get; set; }

        public int PeriodicalId { get; set; }
        public int IssueId { get; set; }

        public int ArticleId { get; set; }

        public string Language { get; set; }

        public string MimeType { get; internal set; }
    }
}
