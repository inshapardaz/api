using System;
using Inshapardaz.Api.Renderers;
using System.Collections.Generic;
using Inshapardaz.Api.Renderers.Dictionary;
using Inshapardaz.Api.View;
using Inshapardaz.Api.View.Dictionary;
using Inshapardaz.Domain.Entities;
using Inshapardaz.Domain.Entities.Dictionary;

namespace Inshapardaz.Api.UnitTests.Fakes.Renderers
{
    public class FakeMeaningRenderer : IRenderMeaning
    {
        private MeaningView _view;
        private readonly List<LinkView> _links = new List<LinkView>();

        public MeaningView Render(Meaning source, int dictionaryId)
        {
            _view.Links = _links;
            return _view;
        }

        public FakeMeaningRenderer WithView(MeaningView view)
        {
            _view = view;
            return this;
        }

        public FakeMeaningRenderer WithLink(string rel, Uri uri)
        {
            _links.Add(new LinkView { Href = uri, Rel = rel });
            return this;
        }
    }
}