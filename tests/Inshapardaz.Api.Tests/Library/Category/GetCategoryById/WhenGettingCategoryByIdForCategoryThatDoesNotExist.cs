using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Domain.Models;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.Categories.GetCategoryById
{
    [TestFixture]
    public class WhenGettingCategoryByIdForCategoryThatDoesNotExist() : TestBase(Role.LibraryAdmin)
    {
        private HttpResponseMessage _response;

        [OneTimeSetUp]
        public async Task Setup() => _response = await Client.GetAsync($"/libraries/{LibraryId}/categories/{-RandomData.Number}");

        [OneTimeTearDown]
        public void Teardown() => Cleanup();

        [Test]
        public void ShouldHaveNotFoundResult() => _response.ShouldBeNotFound();
    }
}
