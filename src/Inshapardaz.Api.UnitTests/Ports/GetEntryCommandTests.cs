using System.Threading.Tasks;
using Inshapardaz.Api.Ports;
using Inshapardaz.Api.UnitTests.Fakes.Renderers;
using Xunit;

namespace Inshapardaz.Api.UnitTests.Ports
{
    public class GetEntryCommandTests
    {
        [Fact]
        public async Task ShouldReturnEntryResponse()
        {
            GetEntryCommandHandler handler = new GetEntryCommandHandler(new FakeEntryRender());

            var command  = new GetEntryRequest();
            await handler.HandleAsync(command);
            Assert.NotNull(command.Result);
        }
    }
}
