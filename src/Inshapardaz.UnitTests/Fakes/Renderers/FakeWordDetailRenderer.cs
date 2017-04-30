using System;
using System.Collections.Generic;
using Inshapardaz.Api.Model;
using Inshapardaz.Api.Renderers;
using Inshapardaz.Domain.Model;

namespace Inshapardaz.Api.UnitTests.Fakes.Renderers
{
    public class FakeWordDetailRenderer : IRenderResponseFromObject<WordDetail, WordDetailView>
    {
        private readonly WordDetailView _response = new WordDetailView();
        private readonly List<LinkView> _links = new List<LinkView>();

        public WordDetailView Render(WordDetail source)
        {
            _response.Links = _links;
            return _response;
        }

        public FakeWordDetailRenderer WithLink(string rel, Uri value)
        {
            _links.Add(new LinkView { Rel = rel, Href = value });
            return this;
        }
    }
}
