using Inshapardaz.Api.Models.Accounts;
using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.Helpers;
using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Accounts.Authenticate
{
    [TestFixture]
    public class WhenUserIsNotVerified : TestBase
    {
        private HttpResponseMessage _response;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var password = RandomData.String;
            var account = AccountBuilder.WithPassword(password).Unverified().Build();

            _response = await Client.PostObject("/accounts/authenticate", new AuthenticateRequest { Email = account.Email, Password = password });
        }

        [Test]
        public void ShouldReturnFailure()
        {
            _response.ShouldBeUnauthorized();
        }
    }
}