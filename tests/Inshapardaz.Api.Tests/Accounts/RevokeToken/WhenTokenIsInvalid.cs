using Inshapardaz.Api.Views.Accounts;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Helpers;
using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Accounts.RevokeToken
{
    [TestFixture]
    public class WhenTokenIsInvalid : TestBase
    {
        private HttpResponseMessage _response;

        public WhenTokenIsInvalid()
            : base(Domain.Models.Role.Admin)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            _response = await Client.PostObject("/accounts/revoke-token", new RevokeTokenRequest() { Token = RandomData.String });
        }

        [Test]
        public void ShouldReturnOk()
        {
            _response.ShouldBeOk();
        }
    }
}
