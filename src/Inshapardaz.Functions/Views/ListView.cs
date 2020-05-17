using System.Collections.Generic;

namespace Inshapardaz.Functions.Views
{
    public class ListView<T> : ViewWithLinks
    {
        public IEnumerable<T> Data { get; set; }
    }
}
