using System.Collections.Generic;

namespace Inshapardaz.Api.View
{
    public abstract class LinkBasedView
    {
        public List<LinkView> Links { get; } = new List<LinkView>();
    }
}
