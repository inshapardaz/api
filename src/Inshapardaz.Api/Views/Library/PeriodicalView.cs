namespace Inshapardaz.Api.Views.Library
{
    public class PeriodicalView : ViewWithLinks
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public int IssueCount { get; set; }
    }
}
