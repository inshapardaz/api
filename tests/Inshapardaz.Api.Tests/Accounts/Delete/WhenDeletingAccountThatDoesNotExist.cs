using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Domain.Models;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Accounts.Delete
{
    [TestFixture]
    public class WhenDeletingAccountThatDoesNotExist() : TestBase(Role.Admin)
    {
        private HttpResponseMessage _response;

        [OneTimeSetUp]
        public async Task Setup() => _response = await Client.DeleteAsync("/accounts/999999999");

        [Test]
        public void ShouldReturnBadRequest() => _response.ShouldBeBadRequest();
    }
}
