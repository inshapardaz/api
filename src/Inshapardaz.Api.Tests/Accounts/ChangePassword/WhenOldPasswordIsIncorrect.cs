using Inshapardaz.Api.Models.Accounts;
using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.Helpers;
using Inshapardaz.Domain.Models;
using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Accounts.ChangePassword
{
    [TestFixture]
    public class WhenOldPasswordIsIncorrect : TestBase
    {
        private HttpResponseMessage _response;
        private string _password;

        public WhenOldPasswordIsIncorrect() : base(Role.Reader)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            _password = RandomData.String;

            _response = await Client.PostObject("/accounts/change-password",
                new ChangePasswordRequest()
                {
                    OldPassword = RandomData.String,
                    Password = _password,
                });
        }

        [Test]
        public void ShouldReturnBadRequest()
        {
            _response.ShouldBeBadRequest();
        }

        [Test]
        public async Task ShouldNotBeAbleToAuthenticateWithNewPassword()
        {
            var response = await Client.PostObject("/accounts/authenticate", new AuthenticateRequest { Email = Account.Email, Password = _password });
            response.ShouldBeUnauthorized();
        }
    }
}