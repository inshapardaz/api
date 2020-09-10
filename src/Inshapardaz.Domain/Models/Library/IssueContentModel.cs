namespace Inshapardaz.Domain.Models.Library
{
    public class IssueContentModel
    {
        public int Id { get; set; }

        public int PeriodicalId { get; set; }
        public int IssueId { get; set; }

        public string ContentUrl { get; set; }

        public string MimeType { get; set; }

        public string Language { get; set; }

        public int FileId { get; internal set; }
    }
}
