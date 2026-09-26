using FluentAssertions;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Domain.Models;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.Book.BookCategories.RemoveCategoryFromBook
{
    public class WhenRemovingCategoryFromBookAsReader() : TestBase(Role.Reader)
    {
        private HttpResponseMessage _response;
        private BookDto _book;
        private List<CategoryDto> _categories;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _categories = CategoryBuilder.WithLibrary(LibraryId).Build(2).ToList();
            _book = BookBuilder.WithLibrary(LibraryId).IsPublic().WithCategories(_categories).Build();

            _response = await Client.DeleteAsync($"/libraries/{LibraryId}/books/{_book.Id}/categories/{_categories[0].Id}");
        }

        [OneTimeTearDown]
        public void Teardown() => Cleanup();

        [Test]
        public void ShouldReturnForbidden() => _response.ShouldBeForbidden();

        [Test]
        public void ShouldNotHaveRemovedCategory()
        {
            CategoryTestRepository.GetCategoriesByBook(_book.Id).Select(c => c.Id)
                .Should().BeEquivalentTo(_categories.Select(c => c.Id));
        }
    }
}
