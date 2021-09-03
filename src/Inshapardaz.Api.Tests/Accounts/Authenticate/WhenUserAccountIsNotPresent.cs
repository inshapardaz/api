using Inshapardaz.Api.Models.Accounts;
using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.Helpers;
using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Accounts.Authenticate
{
    [TestFixture]
    public class WhenUserAccountIsNotPresent : TestBase
    {
        private HttpResponseMessage _response;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _response = await Client.PostObject("/api/accounts/authenticate", new AuthenticateRequest { Email = RandomData.Email, Password = RandomData.String });
        }

        [Test]
        public void ShouldReturnFailure()
        {
            _response.ShouldBeUnauthorized();
        }
    }
}
