using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Api.Views.Library;
using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.Periodical.AddPeriodical
{
    [TestFixture]
    public class WhenAddingPeriodicalAsAnonymousUser : TestBase
    {
        private HttpResponseMessage _response;

        public WhenAddingPeriodicalAsAnonymousUser()
            : base(periodicalsEnabled: true)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var periodical = new PeriodicalView { Title = RandomData.Name, Description = RandomData.Words(20), Frequency = "Weekly", Language = "en" };

            _response = await Client.PostObject($"/libraries/{LibraryId}/periodicals", periodical);
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
