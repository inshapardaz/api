using System;
using System.Collections.Generic;
using Inshapardaz.Api.Model;
using Inshapardaz.Api.Renderers;
using Inshapardaz.Domain.Model;

namespace Inshapardaz.Api.UnitTests.Fakes.Renderers
{
    public class FakeDictionaryRenderer : IRenderResponseFromObject<Dictionary, DictionaryView>
    {
        private readonly DictionaryView _response = new DictionaryView();
        private readonly List<LinkView> _links = new List<LinkView>();

        public DictionaryView Render(Dictionary source)
        {
            _response.Links = _links;
            return _response;
        }

        public FakeDictionaryRenderer WithLink(string rel, Uri value)
        {
            _links.Add(new LinkView { Rel = rel, Href = value });
            return this;
        }

        public FakeDictionaryRenderer WithUserId(string userId)
        {
            _response.UserId = userId;
            return this;
        }
    }
}
