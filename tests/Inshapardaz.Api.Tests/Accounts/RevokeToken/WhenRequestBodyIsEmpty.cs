using System.Text;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Helpers;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Accounts.RevokeToken
{
    [TestFixture]
    public class WhenRequestBodyIsEmpty() : TestBase(Domain.Models.Role.Admin)
    {
        private HttpResponseMessage _response;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var content = new StringContent(string.Empty, Encoding.UTF8, "application/json");
            _response = await Client.PostAsync("/accounts/revoke-token", content);
        }

        [Test]
        public void ShouldReturnBadRequest() => _response.ShouldBeBadRequest();
    }
}
