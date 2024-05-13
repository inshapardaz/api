using Inshapardaz.Api.Views.Accounts;
using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.Helpers;
using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Accounts.RefreshToken
{
    [TestFixture]
    public class WhenRefreshTokenIsInvalid : TestBase
    {
        private HttpResponseMessage _response;

        public WhenRefreshTokenIsInvalid()
            : base(Domain.Models.Role.Reader)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            _response = await Client.PostObject("/accounts/refresh-token", new RefreshTokenRequest { RefreshToken = RandomData.String });
        }

        [Test]
        public void ShouldReturnBadRequest()
        {
            _response.ShouldBeBadRequest();
        }
    }
}
