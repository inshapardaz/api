using Inshapardaz.Api.Models.Accounts;
using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.Helpers;
using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Accounts.RevokeToken
{
    [TestFixture]
    public class WhenUserIsNotAdmin : TestBase
    {
        private HttpResponseMessage _response;

        public WhenUserIsNotAdmin()
            : base(Domain.Models.Role.Reader)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var account = AccountBuilder.Verified().Build();
            var authResponse = await AccountBuilder.Authenticate(Client);

            _response = await Client.PostObject("/accounts/revoke-token", new RevokeTokenRequest() { Token = authResponse.RefreshToken });
        }

        [Test]
        public void ShouldReturnForbidden()
        {
            _response.ShouldBeForbidden();
        }
    }
}