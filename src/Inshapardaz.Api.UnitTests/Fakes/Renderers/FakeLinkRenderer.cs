using System;
using Inshapardaz.Api.Renderers;
using Inshapardaz.Api.View;

namespace Inshapardaz.Api.UnitTests.Fakes.Renderers
{
    public class FakeLinkRenderer : IRenderLink
    {
        public LinkView Render(string methodName, string rel)
        {
            return new LinkView{ Href = new Uri($"http://temp.link/{methodName}"), Rel = rel };
        }

        public LinkView Render(string methodName, string rel, object data)
        {
            return new LinkView { Href = new Uri($"http://temp.link/{methodName}"), Rel = rel };
        }
    }
}
