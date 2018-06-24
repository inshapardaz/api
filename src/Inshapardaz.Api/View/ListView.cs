using System.Collections.Generic;

namespace Inshapardaz.Api.View.Library
{
    public class ListView<T> : LinkBasedView
    {
        public IEnumerable<T> Items { get; set; }

    }
}