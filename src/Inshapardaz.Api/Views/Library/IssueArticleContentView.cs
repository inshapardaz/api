namespace Inshapardaz.Api.Views.Library
{
    public class IssueArticleContentView : ViewWithLinks
    {
        public int Id { get; set; }

        public int PeriodicalId { get; set; }

        public int VolumeNumber { get; set; }

        public int IssueNumber { get; set; }

        public int SequenceNumber { get; set; }

        public string Language { get; set; }

        public string Text { get; set; }
    }
}
