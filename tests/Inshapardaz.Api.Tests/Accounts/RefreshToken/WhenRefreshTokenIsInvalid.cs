using Inshapardaz.Api.Views.Accounts;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Helpers;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Accounts.RefreshToken
{
    [TestFixture]
    public class WhenRefreshTokenIsInvalid() : TestBase(Domain.Models.Role.Reader)
    {
        private HttpResponseMessage _response;

        [OneTimeSetUp]
        public async Task Setup() => _response = await Client.PostObject("/accounts/refresh-token", new RefreshTokenRequest { RefreshToken = RandomData.String });

        [Test]
        public void ShouldReturnBadRequest() => _response.ShouldBeBadRequest();
    }
}
