namespace Inshapardaz.Api.Views
{
    public class LibraryView : ViewWithLinks
    {
        public string Name { get; internal set; }
        public string Language { get; internal set; }
        public bool SupportsPeriodicals { get; internal set; }
    }
}
