using Inshapardaz.Api.Views.Accounts;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Helpers;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Accounts.RevokeToken
{
    [TestFixture]
    public class WhenNoTokenIsSent() : TestBase(Domain.Models.Role.Admin)
    {
        private HttpResponseMessage _response;

        [OneTimeSetUp]
        public async Task Setup() => _response = await Client.PostObject("/accounts/revoke-token", new RevokeTokenRequest());

        [Test]
        public void ShouldReturnBadRequest() => _response.ShouldBeBadRequest();
    }
}
