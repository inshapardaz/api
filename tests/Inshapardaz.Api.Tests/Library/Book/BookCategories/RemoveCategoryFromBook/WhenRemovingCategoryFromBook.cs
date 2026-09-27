using FluentAssertions;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Domain.Models;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.Book.BookCategories.RemoveCategoryFromBook
{
    [TestFixture(Role.Admin)]
    [TestFixture(Role.LibraryAdmin)]
    [TestFixture(Role.Writer)]
    public class WhenRemovingCategoryFromBook(Role role) : TestBase(role)
    {
        private HttpResponseMessage _response;
        private BookDto _book;
        private List<CategoryDto> _categories;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _categories = CategoryBuilder.WithLibrary(LibraryId).Build(3).ToList();
            _book = BookBuilder.WithLibrary(LibraryId).WithCategories(_categories).Build();

            _response = await Client.DeleteAsync($"/libraries/{LibraryId}/books/{_book.Id}/categories/{_categories[0].Id}");
        }

        [OneTimeTearDown]
        public void Teardown() => Cleanup();

        [Test]
        public void ShouldReturnNoContent() => _response.ShouldBeNoContent();

        [Test]
        public void ShouldHaveRemovedOnlyTheSelectedCategory()
        {
            CategoryTestRepository.GetCategoriesByBook(_book.Id).Select(c => c.Id)
                .Should().BeEquivalentTo(_categories.Skip(1).Select(c => c.Id));
        }

        [Test]
        public void ShouldNotHaveDeletedTheCategoryItself()
        {
            CategoryTestRepository.GetCategoryById(LibraryId, _categories[0].Id).Should().NotBeNull();
        }
    }
}
