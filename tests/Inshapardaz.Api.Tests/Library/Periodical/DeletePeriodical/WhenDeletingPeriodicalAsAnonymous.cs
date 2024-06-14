using System.Net.Http;
using System.Threading.Tasks;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Helpers;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.Periodical.DeletePeriodical
{
    [TestFixture]
    public class WhenDeletingPeriodicalAsAnonymous : TestBase
    {
        private HttpResponseMessage _response;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var periodicals = PeriodicalBuilder.WithLibrary(LibraryId).Build(4);
            var expected = periodicals.PickRandom();

            _response = await Client.DeleteAsync($"/libraries/{LibraryId}/periodicals/{expected.Id}");
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldHaveUnauthorizedResult()
        {
            _response.ShouldBeUnauthorized();
        }
    }
}
