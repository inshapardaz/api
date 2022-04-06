namespace Inshapardaz.Api.Views
{
    public class CorrectionView : ViewWithLinks
    {
        public string Language { get; set; }
        public string Profile { get; set; }
        public string IncorrectText { get; set; }
        public string CorrectText { get; set; }
    }
}
