using System;
using System.Collections.Generic;
using Inshapardaz.Api.Renderers;
using Inshapardaz.Api.Renderers.Dictionary;
using Inshapardaz.Api.View;
using Inshapardaz.Api.View.Dictionary;
using Inshapardaz.Domain.Entities;
using Inshapardaz.Domain.Entities.Dictionary;

namespace Inshapardaz.Api.UnitTests.Fakes.Renderers
{
    public class FakeWordRenderer : IRenderWord
    {
        private readonly WordView _response = new WordView();
        private readonly List<LinkView> _links = new List<LinkView>();

        public WordView Render(Word source, int dictionaryId)
        {
            _response.Links = _links;
            return _response;
        }

        public FakeWordRenderer WithLink(string rel, Uri value)
        {
            _links.Add(new LinkView { Rel = rel, Href = value });
            return this;
        }
    }
}
