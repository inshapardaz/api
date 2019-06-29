using Inshapardaz.Functions.Views;

namespace Inshapardaz.Functions.View.Library
{
    public class CategoryView : ViewWithLinks
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int BookCount { get; internal set; }
    }
}
