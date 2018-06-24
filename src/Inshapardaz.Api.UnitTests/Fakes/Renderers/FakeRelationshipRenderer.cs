using System;
using Inshapardaz.Api.Renderers;
using System.Collections.Generic;
using System.Linq;
using Inshapardaz.Api.Renderers.Dictionary;
using Inshapardaz.Api.View;
using Inshapardaz.Api.View.Dictionary;
using Inshapardaz.Domain.Entities;
using Inshapardaz.Domain.Entities.Dictionary;

namespace Inshapardaz.Api.UnitTests.Fakes.Renderers
{
    public class FakeRelationshipRenderer : IRenderRelation
    {
        private RelationshipView _view;
        private readonly List<LinkView> _links = new List<LinkView>();

        public RelationshipView Render(WordRelation source, int dictionaryId, long wordId)
        {
            _view.Links = _links;
            if (_view.Links == null || !_view.Links.Any())
            {
                _view.Links = new List<LinkView> { new LinkView { Rel = "self", Href = new Uri("http://link.test/123") } };
            }
            return _view;
        }

        public FakeRelationshipRenderer WithView(RelationshipView view)
        {
            _view = view;
            return this;
        }

        public FakeRelationshipRenderer WithLink(string rel, Uri uri)
        {
            _links.Add(new LinkView { Href = uri, Rel = rel });
            return this;
        }
    }
}