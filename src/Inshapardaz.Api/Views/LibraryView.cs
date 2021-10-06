namespace Inshapardaz.Api.Views
{
    public class LibraryView : ViewWithLinks
    {
        public int Id { get; set; }
        public string OwnerEmail { get; set; }
        public string Name { get; set; }
        public string Language { get; set; }
        public bool SupportsPeriodicals { get; set; }
        public string PrimaryColor { get; set; }
        public string SecondaryColor { get; set; }
    }
}
