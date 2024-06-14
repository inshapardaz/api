using Inshapardaz.Api.Views.Accounts;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Helpers;
using FluentAssertions;
using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Accounts.RefreshToken
{
    [TestFixture]
    public class WhenRefreshTokenAsSuperAdmin : TestBase
    {
        private HttpResponseMessage _response;
        private AuthenticateResponse _authResponse;
        private AuthenticateResponse _refreshResponse;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var library = LibraryBuilder.Build();
            var account = AccountBuilder.As(Domain.Models.Role.Admin).Verified().InLibrary(library.Id).Build();
            _authResponse = await AccountBuilder.Authenticate(Client, account.Email);
            AuthenticateClientWithToken(_authResponse.AccessToken);

            _response = await Client.PostObject("/accounts/refresh-token", new RefreshTokenRequest { RefreshToken = _authResponse.RefreshToken });
            _refreshResponse = await _response.GetContent<AuthenticateResponse>();
        }

        [Test]
        public void ShouldReturnOK()
        {
            _response.ShouldBeOk();
        }

        [Test]
        public void ShouldReturnNewRefreshToken()
        {
            _refreshResponse.RefreshToken.Should().NotBeNullOrEmpty()
                                 .Should().NotBe(_authResponse.RefreshToken);
        }

        [Test]
        public void ShouldReturnNewAccessToken()
        {
            _refreshResponse.AccessToken.Should().NotBeNullOrEmpty()
                                 .Should().NotBe(_authResponse.AccessToken);
        }
    }
}
