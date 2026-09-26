using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Helpers;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.Book.BookCategories.SetBookCategories
{
    public class WhenSettingBookCategoriesAsUnauthorized : TestBase
    {
        private HttpResponseMessage _response;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var category = CategoryBuilder.WithLibrary(LibraryId).Build();
            var book = BookBuilder.WithLibrary(LibraryId).IsPublic().Build();

            _response = await Client.PutObject($"/libraries/{LibraryId}/books/{book.Id}/categories", new List<int> { category.Id });
        }

        [OneTimeTearDown]
        public void Teardown() => Cleanup();

        [Test]
        public void ShouldReturnUnauthorized() => _response.ShouldBeUnauthorized();
    }
}
