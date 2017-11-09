using Inshapardaz.Api.Renderers;
using Inshapardaz.Api.UnitTests.Fakes.Renderers;
using Inshapardaz.Api.View;
using Shouldly;
using Xunit;

namespace Inshapardaz.Api.UnitTests.Renderers
{
    public class EntryRendererTests
    {
        public class WhenGettingEntry
        {

            private readonly EntryView _view;

            public WhenGettingEntry()
            {
                var linkRenderer = new FakeLinkRenderer();
                var renderer = new EntryRenderer(linkRenderer);

                _view = renderer.Render();
            }

            [Fact]
            public void ShouldReturnSelfLink()
            {
                _view.Links.ShouldContain(x => x.Rel == RelTypes.Self);
            }

            [Fact]
            public void ShouldReturnDictionariesLink()
            {
                _view.Links.ShouldContain(x => x.Rel == RelTypes.Dictionaries);
            }

            [Fact]
            public void ShouldReturnThesaurusesLink()
            {
                _view.Links.ShouldContain(x => x.Rel == RelTypes.Thesauruses);
            }

            [Fact]
            public void ShouldReturnLanguagesLink()
            {
                _view.Links.ShouldContain(x => x.Rel == RelTypes.Languages);
            }

            [Fact]
            public void ShouldReturnAttributesLink()
            {
                _view.Links.ShouldContain(x => x.Rel == RelTypes.Attributes);
            }

            [Fact]
            public void ShouldReturnRelationshipTypesLink()
            {
                _view.Links.ShouldContain(x => x.Rel == RelTypes.RelationshipTypes);
            }
        }
    }
}