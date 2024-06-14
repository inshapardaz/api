using Inshapardaz.Api.Views.Accounts;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.DataHelpers;
using Inshapardaz.Api.Tests.Framework.Helpers;
using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Accounts.RefreshToken
{
    [TestFixture]
    public class WhenRefreshTokenIsRevoked : TestBase
    {
        private HttpResponseMessage _response;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var account = AccountBuilder.Verified().Build();
            var auth = await AccountBuilder.Authenticate(Client, account.Email);
            AuthenticateClientWithToken(auth.AccessToken);
            var _oldToken = auth.RefreshToken;
            DatabaseConnection.RevokeRefreshToken(_oldToken);
            _response = await Client.PostObject("/accounts/refresh-token", new RefreshTokenRequest { RefreshToken = RandomData.String });
        }

        [Test]
        public void ShouldReturnBadRequest()
        {
            _response.ShouldBeBadRequest();
        }
    }
}
