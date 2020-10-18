using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.Helpers;
using Inshapardaz.Api.Views.Library;
using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.Periodical.AddPeriodical
{
    [TestFixture]
    public class WhenAddingPeriodicalToLibraryNotSupportingPeriodicals : TestBase
    {
        private HttpResponseMessage _response;

        public WhenAddingPeriodicalToLibraryNotSupportingPeriodicals()
            : base(Domain.Adapters.Permission.LibraryAdmin, false)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var periodical = new PeriodicalView { Title = Random.Name, Description = Random.Words(20) };

            _response = await Client.PostObject($"/library/{LibraryId}/periodicals", periodical);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldHaveBadRequestResult()
        {
            _response.ShouldBeBadRequest();
        }
    }
}
