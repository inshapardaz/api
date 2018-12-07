using Inshapardaz.Api.Renderers;
using Inshapardaz.Api.UnitTests.Fakes.Helpers;
using Inshapardaz.Api.UnitTests.Fakes.Renderers;
using Inshapardaz.Api.View;
using NUnit.Framework;
using Shouldly;

namespace Inshapardaz.Api.UnitTests.Renderers
{
    public class EntryRendererTests
    {
        [TestFixture]
        public class WhenGettingEntry
        {

            private readonly EntryView _view;

            public WhenGettingEntry()
            {
                var linkRenderer = new FakeLinkRenderer();
                var renderer = new EntryRenderer(linkRenderer, new FakeUserHelper());

                _view = renderer.Render();
            }

            [Test]
            public void ShouldReturnSelfLink()
            {
                _view.Links.ShouldContain(x => x.Rel == RelTypes.Self);
            }

            [Test]
            public void ShouldReturnDictionariesLink()
            {
                _view.Links.ShouldContain(x => x.Rel == RelTypes.Dictionaries);
            }

            [Test]
            public void ShouldReturnThesaurusesLink()
            {
                _view.Links.ShouldContain(x => x.Rel == RelTypes.Thesauruses);
            }

            [Test]
            public void ShouldReturnLanguagesLink()
            {
                _view.Links.ShouldContain(x => x.Rel == RelTypes.Languages);
            }

            [Test]
            public void ShouldReturnAttributesLink()
            {
                _view.Links.ShouldContain(x => x.Rel == RelTypes.Attributes);
            }

            [Test]
            public void ShouldReturnRelationshipTypesLink()
            {
                _view.Links.ShouldContain(x => x.Rel == RelTypes.RelationshipTypes);
            }
        }
    }
}