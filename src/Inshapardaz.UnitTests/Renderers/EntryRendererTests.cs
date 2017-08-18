using System.Linq;
using Inshapardaz.Api.Renderers;
using Inshapardaz.Api.View;
using Moq;
using Xunit;

namespace Inshapardaz.Api.UnitTests.Renderers
{
    public class EntryRendererTests
    {
        EntryRenderer _renderer;
        EntryView _view;
        Mock<IRenderLink> _linkRenderer;

        public EntryRendererTests()
        {
            _linkRenderer = new Mock<IRenderLink>();
            _linkRenderer.Setup(x => x.Render(It.IsAny<string>(), It.IsAny<string>()))
                         .Returns<string, string>((m, r) => new LinkView() { Rel = r });
            _linkRenderer.Setup(x => x.Render(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>()))
                         .Returns<string, string, object>((m, r, o) => new LinkView() { Rel = r });

            _renderer = new EntryRenderer(_linkRenderer.Object);
            _view = _renderer.Render();
        }

        [Fact]
        public void ShouldReturnSelfLink()
        {
            Assert.NotNull(_view.Links.SingleOrDefault(x => x.Rel == RelTypes.Self));
        }

        [Fact]
        public void ShouldReturnDictionariesLink()
        {
            Assert.NotNull(_view.Links.SingleOrDefault(x => x.Rel == RelTypes.Dictionaries));
        }

        [Fact]
        public void ShouldReturnThesaurusesLink()
        {
            Assert.NotNull(_view.Links.SingleOrDefault(x => x.Rel == RelTypes.Thesauruses));
        }

        [Fact]
        public void ShouldReturnLanguagesLink()
        {
            Assert.NotNull(_view.Links.SingleOrDefault(x => x.Rel == RelTypes.Languages));
        }

        [Fact]
        public void ShouldReturnAttributesLink()
        {
            Assert.NotNull(_view.Links.SingleOrDefault(x => x.Rel == RelTypes.Attributes));
        }

        [Fact]
        public void ShouldReturnRelationshipTypesLink()
        {
            Assert.NotNull(_view.Links.SingleOrDefault(x => x.Rel == RelTypes.RelationshipTypes));
        }
    }
}
