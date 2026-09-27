using FluentAssertions;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Api.Views;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Models;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.Book.BookCategories.GetBookCategories
{
    [TestFixture(Role.Admin)]
    [TestFixture(Role.LibraryAdmin)]
    [TestFixture(Role.Writer)]
    [TestFixture(Role.Reader)]
    public class WhenGettingBookCategories(Role role) : TestBase(role)
    {
        private HttpResponseMessage _response;
        private ListView<CategoryView> _view;
        private BookDto _book;
        private List<CategoryDto> _categories;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _categories = CategoryBuilder.WithLibrary(LibraryId).Build(3).ToList();
            var otherCategory = CategoryBuilder.WithLibrary(LibraryId).Build();
            _book = BookBuilder.WithLibrary(LibraryId).IsPublic().WithCategories(_categories).Build();

            _response = await Client.GetAsync($"/libraries/{LibraryId}/books/{_book.Id}/categories");
            _view = await _response.GetContent<ListView<CategoryView>>();
        }

        [OneTimeTearDown]
        public void Teardown() => Cleanup();

        [Test]
        public void ShouldReturnOk() => _response.ShouldBeOk();

        [Test]
        public void ShouldReturnOnlyCategoriesOfTheBook()
        {
            _view.Data.Select(c => c.Id).Should().BeEquivalentTo(_categories.Select(c => c.Id));
        }
    }
}
