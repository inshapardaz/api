using Inshapardaz.Api.Models.Accounts;
using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.Helpers;
using Inshapardaz.Domain.Models;
using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Accounts.ChangePassword
{
    [TestFixture(Role.Admin)]
    [TestFixture(Role.LibraryAdmin)]
    [TestFixture(Role.Reader)]
    public class WhenChangingPassword : TestBase
    {
        private HttpResponseMessage _response;
        private string _password = RandomData.String;

        public WhenChangingPassword(Role role) : base(role)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            _response = await Client.PostObject("/accounts/change-password",
                new ChangePasswordRequest()
                {
                    OldPassword = AccountBuilder._password,
                    Password = _password,
                });
        }

        [Test]
        public void ShouldReturnOk()
        {
            _response.ShouldBeOk();
        }

        [Test]
        public async Task ShouldBeAbleToAuthenticateWithNewPassword()
        {
            var response = await Client.PostObject("/accounts/authenticate", new AuthenticateRequest { Email = Account.Email, Password = _password });
            response.ShouldBeOk();
        }
    }
}