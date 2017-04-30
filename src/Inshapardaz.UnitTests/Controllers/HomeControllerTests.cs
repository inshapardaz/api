using Inshapardaz.Api.Controllers;
using Inshapardaz.Api.Model;
using Inshapardaz.Api.UnitTests.Fakes.Renderers;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace Inshapardaz.Api.UnitTests.Controllers
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
                var objectResult = controller.Index() as ObjectResult;
                _response = objectResult.Value as EntryView;
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
