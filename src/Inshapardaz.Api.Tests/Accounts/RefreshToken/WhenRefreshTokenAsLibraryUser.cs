using Inshapardaz.Api.Models.Accounts;
using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.Helpers;
using FluentAssertions;
using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Accounts.RefreshToken
{
    [TestFixture]
    public class WhenRefreshTokenAsLibraryUser : TestBase
    {
        private HttpResponseMessage _response;
        private AuthenticateResponse _authResponse;
        private AuthenticateResponse _refreshResponse;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var library = LibraryBuilder.Build();
            var account = AccountBuilder.As(Domain.Models.Role.Reader).Verified().InLibrary(library.Id).Build();
            _authResponse = await AccountBuilder.Authenticate(Client, account.Email);

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
                                 .And.Should().NotBe(_authResponse.RefreshToken);
        }

        [Test]
        public void ShouldReturnNewAccessToken()
        {
            _refreshResponse.JwtToken.Should().NotBeNullOrEmpty()
                                 .And.Should().NotBe(_authResponse.JwtToken);
        }
    }
}