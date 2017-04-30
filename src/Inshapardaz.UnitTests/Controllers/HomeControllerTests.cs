using Inshapardaz.Controllers;
using Inshapardaz.Model;
using Inshapardaz.UnitTests.Fakes.Renderers;
using Xunit;

namespace Inshapardaz.UnitTests.Controllers
{
    public class HomeControllerTests
    {
        public class WhenGettingIndex
        {
            private readonly EntryView _response;
            private readonly FakeEntryRender _renderer;

            public WhenGettingIndex()
            {
                _renderer = new FakeEntryRender();
                var controller = new HomeController(_renderer);
                _response = controller.Index();
            }

            [Fact]
            public void ShouldReturnSomeResponse()
            {
                Assert.NotNull(_response);
            }
                
            [Fact]
            public void ShouldUseRendererToRenderResponse()
            {
                Assert.True(_renderer.WasRendered);
            }
        }
    }
}
