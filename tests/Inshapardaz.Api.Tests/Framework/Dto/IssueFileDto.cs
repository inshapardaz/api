namespace Inshapardaz.Api.Tests.Framework.Dto
{
    public class IssueFileDto
    {
        public int Id { get; set; }

        public int IssueId { get; set; }

        public int FileId { get; set; }

        public string Language { get; set; }
        public object VolumeNumber { get; internal set; }
        public object IssueNumber { get; internal set; }
    }
}
