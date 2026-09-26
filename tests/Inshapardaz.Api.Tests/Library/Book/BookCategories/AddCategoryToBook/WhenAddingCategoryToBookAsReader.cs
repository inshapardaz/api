using FluentAssertions;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Domain.Models;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.Book.BookCategories.AddCategoryToBook
{
    public class WhenAddingCategoryToBookAsReader() : TestBase(Role.Reader)
    {
        private HttpResponseMessage _response;
        private BookDto _book;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var category = CategoryBuilder.WithLibrary(LibraryId).Build();
            _book = BookBuilder.WithLibrary(LibraryId).IsPublic().Build();

            _response = await Client.PostAsync($"/libraries/{LibraryId}/books/{_book.Id}/categories/{category.Id}", null);
        }

        [OneTimeTearDown]
        public void Teardown() => Cleanup();

        [Test]
        public void ShouldReturnForbidden() => _response.ShouldBeForbidden();

        [Test]
        public void ShouldNotHaveAddedCategory() => CategoryTestRepository.GetCategoriesByBook(_book.Id).Should().BeEmpty();
    }
}
