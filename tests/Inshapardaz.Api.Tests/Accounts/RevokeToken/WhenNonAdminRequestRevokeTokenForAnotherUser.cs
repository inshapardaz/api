using Inshapardaz.Api.Views.Accounts;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Helpers;
using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Accounts.RevokeToken
{
    [TestFixture(Domain.Models.Role.LibraryAdmin)]
    [TestFixture(Domain.Models.Role.Writer)]
    [TestFixture(Domain.Models.Role.Reader)]
    public class WhenNonAdminRequestRevokeTokenForAnotherUser : TestBase
    {
        private HttpResponseMessage _response;

        public WhenNonAdminRequestRevokeTokenForAnotherUser(Domain.Models.Role role)
            : base(role)
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
        public void ShouldReturnUnauthorised()
        {
            _response.ShouldBeUnauthorized();
        }
    }
}
