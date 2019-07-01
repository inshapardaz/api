using Inshapardaz.Functions.Views;

namespace Inshapardaz.Functions.View.Library
{
    public class SeriesView : ViewWithLinks
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public int BookCount { get; set; }
    }
}
