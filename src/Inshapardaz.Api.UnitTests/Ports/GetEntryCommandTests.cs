using System.Threading.Tasks;
using Inshapardaz.Api.Adapters;
using Inshapardaz.Api.UnitTests.Fakes.Renderers;
using NUnit.Framework;

namespace Inshapardaz.Api.UnitTests.Ports
{
    [TestFixture]
    public class GetEntryCommandTests
    {
        [Test]
        public async Task ShouldReturnEntryResponse()
        {
            var handler = new GetEntryRequestHandler(new FakeEntryRender());

            var command  = new GetEntryRequest();
            await handler.HandleAsync(command);
            Assert.NotNull(command.Result);
        }
    }
}
