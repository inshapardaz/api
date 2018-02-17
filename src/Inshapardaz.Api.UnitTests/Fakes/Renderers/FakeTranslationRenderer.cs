using System;
using Inshapardaz.Api.Renderers;
using System.Collections.Generic;
using System.Linq;
using Inshapardaz.Api.View;
using Inshapardaz.Domain.Entities;

namespace Inshapardaz.Api.UnitTests.Fakes.Renderers
{
    public class FakeTranslationRenderer : IRenderTranslation
    {
        private TranslationView _view;
        private readonly List<LinkView> _links = new List<LinkView>();

        public TranslationView Render(Translation source, int dictionaryId)
        {
            _view.Links = _links;
            if (_view.Links == null || !_view.Links.Any())
            {
                _view.Links = new List<LinkView> { new LinkView { Rel = "self", Href = new Uri("http://link.test/123") } };
            }
            return _view;
        }

        public FakeTranslationRenderer WithView(TranslationView view)
        {
            _view = view;
            return this;
        }

        public FakeTranslationRenderer WithLink(string rel, Uri uri)
        {
            _links.Add(new LinkView { Href = uri, Rel = rel });
            return this;
        }
    }
}