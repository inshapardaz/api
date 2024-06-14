using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Helpers;
using FluentAssertions;
using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;
using Inshapardaz.Api.Views.Accounts;

namespace Inshapardaz.Api.Tests.Accounts.Authenticate
{
    [TestFixture]
    public class WhenCredentialsAreCorrect : TestBase
    {
        private HttpResponseMessage _response;
        private AuthenticateResponse _authenticateResponse;
        private AccountDto _account;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var password = RandomData.String;
            _account = AccountBuilder.WithPassword(password).Verified().Build();

            _response = await Client.PostObject("/accounts/authenticate", new AuthenticateRequest { Email = _account.Email, Password = password });
            _authenticateResponse = await _response.GetContent<AuthenticateResponse>();
        }

        [Test]
        public void ShouldReturnOk()
        {
            _response.ShouldBeOk();
        }

        [Test]
        public void ShouldContainRefreshToken()
        {
            _authenticateResponse.RefreshToken.Should().NotBeNullOrEmpty();
        }

        [Test]
        public void ShouldContainAccessToken()
        {
            _authenticateResponse.AccessToken.Should().NotBeNullOrEmpty();
        }

        [Test]
        public void ShouldContainCorrectEmail()
        {
            _authenticateResponse.Email.Should().Be(_account.Email);
        }

        [Test]
        public void ShouldContainName()
        {
            _authenticateResponse.Name.Should().Be(_account.Name);
        }
    }
}
