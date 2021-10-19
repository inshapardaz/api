using Inshapardaz.Api.Models.Accounts;
using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.Helpers;
using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Accounts.RevokeToken
{
    [TestFixture(Domain.Models.Role.LibraryAdmin)]
    [TestFixture(Domain.Models.Role.Admin)]
    [TestFixture(Domain.Models.Role.Writer)]
    [TestFixture(Domain.Models.Role.Reader)]
    public class WhenUserRequestRevokeToken : TestBase
    {
        private HttpResponseMessage _response;

        public WhenUserRequestRevokeToken(Domain.Models.Role role)
            : base(role)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var authResponse = await AccountBuilder.Authenticate(Client, Account.Email);

            _response = await Client.PostObject("/accounts/revoke-token", new RevokeTokenRequest() { Token = authResponse.RefreshToken });
        }

        [Test]
        public void ShouldReturnOk()
        {
            _response.ShouldBeOk();
        }
    }
}
