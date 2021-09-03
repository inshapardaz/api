using Inshapardaz.Api.Models.Accounts;
using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.Helpers;
using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Accounts.ResetPassword
{
    [TestFixture]
    public class WhenResetingTokenDoesNotExist : TestBase
    {
        private HttpResponseMessage _response;
        private string _password;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _password = RandomData.String;

            _response = await Client.PostObject("/accounts/reset-password",
                new ResetPasswordRequest()
                {
                    Token = RandomData.String,
                    Password = _password
                });
        }

        [Test]
        public void ShouldReturnBadRequest()
        {
            _response.ShouldBeBadRequest();
        }
    }
}