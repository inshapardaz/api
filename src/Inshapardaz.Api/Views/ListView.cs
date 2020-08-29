using System.Collections.Generic;

namespace Inshapardaz.Api.Views
{
    public class ListView<T> : ViewWithLinks
    {
        public IEnumerable<T> Data { get; set; }
    }
}