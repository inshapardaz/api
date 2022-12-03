namespace Inshapardaz.Api.Views
{
    public class CorrectionView : ViewWithLinks
    {
        public long Id { get; set; }
        public string Language { get; set; }
        public string Profile { get; set; }
        public string IncorrectText { get; set; }
        public string CorrectText { get; set; }
        public bool CompleteWord { get; set; }
    }

    public class CorrectionSimpleView
    {
        public string IncorrectText { get; set; }
        public string CorrectText { get; set; }
        public bool CompleteWord { get; set; }
    }
}
