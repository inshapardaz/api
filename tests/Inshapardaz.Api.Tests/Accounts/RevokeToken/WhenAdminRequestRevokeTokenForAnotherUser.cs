using Inshapardaz.Api.Models.Accounts;
using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.Helpers;
using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Accounts.RevokeToken
{
    [TestFixture]
    public class WhenAdminRequestRevokeTokenForAnotherUser : TestBase
    {
        private HttpResponseMessage _response;

        public WhenAdminRequestRevokeTokenForAnotherUser()
            : base(Domain.Models.Role.Admin)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var account = AccountBuilder.Verified().Build();
            var authResponse = await AccountBuilder.Authenticate(Client, account.Email);

            _response = await Client.PostObject("/accounts/revoke-token", new RevokeTokenRequest() { Token = authResponse.RefreshToken });
        }

        [Test]
        public void ShouldReturnOk()
        {
            _response.ShouldBeOk();
        }
    }
}
