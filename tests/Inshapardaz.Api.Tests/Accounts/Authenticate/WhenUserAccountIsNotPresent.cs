using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Api.Views.Accounts;
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
            _response = await Client.PostObject("/accounts/authenticate", new AuthenticateRequest { Email = RandomData.Email, Password = RandomData.String });
        }

        [Test]
        public void ShouldReturnFailure()
        {
            _response.ShouldBeUnauthorized();
        }
    }
}
