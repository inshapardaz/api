using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Domain.Models;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.BookShelf.GetBookShelfById
{
    [TestFixture]
    public class WhenGettingBookShelfByIdThatDoesNotExist() : TestBase(Role.Writer)
    {
        private HttpResponseMessage _response;

        [OneTimeSetUp]
        public async Task Setup() => _response = await Client.GetAsync($"/libraries/{LibraryId}/bookshelves/{-RandomData.Number}");

        [OneTimeTearDown]
        public void Teardown() => Cleanup();

        [Test]
        public void ShouldHaveNotFoundResult() => _response.ShouldBeNotFound();
    }
}
