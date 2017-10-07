using System;
using Inshapardaz.Api.Renderers;
using Inshapardaz.Domain.Model;
using System.Collections.Generic;
using Inshapardaz.Api.View;
using Inshapardaz.Domain.Database.Entities;

namespace Inshapardaz.Api.UnitTests.Fakes.Renderers
{
    public class FakeMeaningRenderer : IRenderResponseFromObject<Meaning, MeaningView>
    {
        private MeaningView _view;
        private readonly List<LinkView> _links = new List<LinkView>();

        public MeaningView Render(Meaning source)
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