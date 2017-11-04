using System;
using System.Collections.Generic;
using Inshapardaz.Api.Renderers;
using Inshapardaz.Api.View;
using Inshapardaz.Domain.Database.Entities;

namespace Inshapardaz.Api.UnitTests.Fakes.Renderers
{
    public class FakeDictionaryRenderer : IRenderDictionary
    {
        private readonly DictionaryView _response = new DictionaryView();
        private readonly List<LinkView> _links = new List<LinkView>();

        public DictionaryView Render(Dictionary source, int wordCount)
        {
            _response.Links = _links;
            _response.WordCount = wordCount;
            return _response;
        }

        public FakeDictionaryRenderer WithLink(string rel, Uri value)
        {
            _links.Add(new LinkView { Rel = rel, Href = value });
            return this;
        }

        public FakeDictionaryRenderer WithUserId(Guid userId)
        {
            _response.UserId = userId;
            return this;
        }
    }
}
