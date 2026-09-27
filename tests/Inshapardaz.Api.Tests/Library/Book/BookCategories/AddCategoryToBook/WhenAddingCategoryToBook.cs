using FluentAssertions;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Api.Views;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Models;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.Book.BookCategories.AddCategoryToBook
{
    [TestFixture(Role.Admin)]
    [TestFixture(Role.LibraryAdmin)]
    [TestFixture(Role.Writer)]
    public class WhenAddingCategoryToBook(Role role) : TestBase(role)
    {
        private HttpResponseMessage _response;
        private ListView<CategoryView> _view;
        private BookDto _book;
        private List<CategoryDto> _existing;
        private CategoryDto _added;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _existing = CategoryBuilder.WithLibrary(LibraryId).Build(2).ToList();
            _added = CategoryBuilder.WithLibrary(LibraryId).Build();
            _book = BookBuilder.WithLibrary(LibraryId).WithCategories(_existing).Build();

            _response = await Client.PostAsync($"/libraries/{LibraryId}/books/{_book.Id}/categories/{_added.Id}", null);
            _view = await _response.GetContent<ListView<CategoryView>>();
        }

        [OneTimeTearDown]
        public void Teardown() => Cleanup();

        [Test]
        public void ShouldReturnOk() => _response.ShouldBeOk();

        [Test]
        public void ShouldReturnAllCategoriesIncludingNewOne()
        {
            _view.Data.Select(c => c.Id).Should().BeEquivalentTo(_existing.Select(c => c.Id).Append(_added.Id));
        }

        [Test]
        public void ShouldKeepExistingCategoriesAndSaveNewOne()
        {
            CategoryTestRepository.GetCategoriesByBook(_book.Id).Select(c => c.Id)
                .Should().BeEquivalentTo(_existing.Select(c => c.Id).Append(_added.Id));
        }
    }
}
