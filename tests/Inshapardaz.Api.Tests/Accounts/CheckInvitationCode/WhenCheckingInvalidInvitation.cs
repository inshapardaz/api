using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Helpers;
using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Accounts.CheckInvitationCode
{
    [TestFixture]
    public class WhenCheckingInvalidInvitation : TestBase
    {
        private HttpResponseMessage _response;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _response = await Client.GetAsync($"/accounts/invitation/{RandomData.String}");
        }

        [Test]
        public void ShouldReturnNotFound()
        {
            _response.ShouldBeNotFound();
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            Cleanup();
        }
    }
}