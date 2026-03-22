using Inshapardaz.Api.Views.Accounts;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Helpers;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Accounts.RevokeToken
{
    [TestFixture(Domain.Models.Role.LibraryAdmin)]
    [TestFixture(Domain.Models.Role.Admin)]
    [TestFixture(Domain.Models.Role.Writer)]
    [TestFixture(Domain.Models.Role.Reader)]
    public class WhenUserRequestRevokeToken(Domain.Models.Role role) : TestBase(role)
    {
        private HttpResponseMessage _response;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var authResponse = await AccountBuilder.Authenticate(Client, Account.Email);

            _response = await Client.PostObject("/accounts/revoke-token", new RevokeTokenRequest() { Token = authResponse.RefreshToken });
        }

        [Test]
        public void ShouldReturnOk() => _response.ShouldBeOk();
    }
}
