using Inshapardaz.Api.Views.Accounts;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Helpers;
using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Accounts.ForgotPassword
{
    [TestFixture]
    public class WhenAccountDoesNotExists : TestBase
    {
        private HttpResponseMessage _response;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _response = await Client.PostObject("/accounts/forgot-password", new ForgotPasswordRequest() { Email = RandomData.Email });
        }

        [Test]
        public void ShouldReturnOk()
        {
            _response.ShouldBeOk();
        }
    }
}