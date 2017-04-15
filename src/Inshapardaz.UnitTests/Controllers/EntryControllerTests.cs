using Inshapardaz.Controllers;
using Inshapardaz.Model;
using Inshapardaz.UnitTests.Fakes.Renderers;
using System.Linq;
using Xunit;

namespace Inshapardaz.UnitTests.Controllers
{
    public class EntryControllerTests
    {
        public class Get
        {
            EntryController controller;
            EntryView _response;
            FakeEntryRender _renderer;

            public Get()
            {
                _renderer = new FakeEntryRender();
                controller = new EntryController(_renderer, new FakeDictionaryEntryRender(), new FakeIndexRenderer());
                _response = controller.Get();
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
