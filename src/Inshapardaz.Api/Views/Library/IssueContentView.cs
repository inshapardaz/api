namespace Inshapardaz.Api.Views.Library
{
    public class IssueContentView : ViewWithLinks
    {
        public int Id { get; set; }

        public int PeriodicalId { get; set; }
        public int IssueId { get; set; }

        public string MimeType { get; set; }

        public string Language { get; set; }
    }
}
