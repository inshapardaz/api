using Inshapardaz.Api.Views.Accounts;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Helpers;
using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Accounts.Authenticate
{
    [TestFixture]
    public class WhenCredentialsAreIncorrect : TestBase
    {
        private HttpResponseMessage _response;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var password = RandomData.String;
            var account = AccountBuilder.WithPassword(password).Verified().Build();

            _response = await Client.PostObject("/accounts/authenticate", new AuthenticateRequest { Email = account.Email, Password = RandomData.String });
        }

        [Test]
        public void ShouldReturnFailure()
        {
            _response.ShouldBeUnauthorized();
        }
    }
}
