using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Helpers;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Accounts.Delete
{
    [TestFixture]
    public class WhenDeletingOwnAccount() : TestBase(Domain.Models.Role.Reader)
    {
        private HttpResponseMessage _response;

        [OneTimeSetUp]
        public async Task Setup() => _response = await Client.DeleteAsync($"/accounts/{AccountId}");

        [Test]
        public void ShouldReturnOk() => _response.ShouldBeOk();

        [Test]
        public void ShouldAnonymizeAccount() => AccountAssert.ShouldBeAnonymized(AccountId);
    }
}
