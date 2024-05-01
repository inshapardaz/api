namespace Inshapardaz.Api.Tests.Dto
{
    public class CorrectionDto
    {
        public long Id { get; set; }

        public string Language { get; set; }

        public string Profile { get; set; }

        public string IncorrectText { get; set; }

        public string CorrectText { get; set; }

        public long Usage { get; set; }

        public bool CompleteWord { get; set; }
    }
}
