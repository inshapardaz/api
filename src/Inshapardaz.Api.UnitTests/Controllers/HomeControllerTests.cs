using System.Threading;
using Inshapardaz.Api.Controllers;
using Inshapardaz.Api.Ports;
using Inshapardaz.Api.UnitTests.Fakes.Renderers;
using Inshapardaz.Api.View;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Paramore.Brighter;
using Xunit;

namespace Inshapardaz.Api.UnitTests.Controllers
{
    public class HomeControllerTests
    {
        public class WhenGettingIndex
        {
            private readonly Mock<IAmACommandProcessor> _mockCommandProcessor;

            private readonly EntryView _response;
            private readonly FakeEntryRender _renderer;

            public WhenGettingIndex()
            {
                _mockCommandProcessor = new Mock<IAmACommandProcessor>();
                _renderer = new FakeEntryRender();
                var controller = new HomeController(_mockCommandProcessor.Object, null);
                var result = controller.Index().Result as OkObjectResult;
                _response = result?.Value as EntryView;
            }

            [Fact]
            public void ShouldExecuteCorrectCommand()
            {
                
            }
        }
    }
}
