using FluentAssertions;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Api.Views;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Models;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.Book.BookCategories.SetBookCategories
{
    [TestFixture(Role.Admin)]
    [TestFixture(Role.LibraryAdmin)]
    [TestFixture(Role.Writer)]
    public class WhenSettingBookCategoriesWithPermission(Role role) : TestBase(role)
    {
        private HttpResponseMessage _response;
        private ListView<CategoryView> _view;
        private BookDto _book;
        private List<CategoryDto> _expected;
        private CategoryDto _removed;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var existing = CategoryBuilder.WithLibrary(LibraryId).Build(2).ToList();
            var added = CategoryBuilder.WithLibrary(LibraryId).Build(2).ToList();
            _book = BookBuilder.WithLibrary(LibraryId).WithCategories(existing).Build();

            _removed = existing[1];
            _expected = new List<CategoryDto> { existing[0], added[0], added[1] };

            _response = await Client.PutObject($"/libraries/{LibraryId}/books/{_book.Id}/categories", _expected.Select(c => c.Id).ToList());
            _view = await _response.GetContent<ListView<CategoryView>>();
        }

        [OneTimeTearDown]
        public void Teardown() => Cleanup();

        [Test]
        public void ShouldReturnOk() => _response.ShouldBeOk();

        [Test]
        public void ShouldReturnNewCategorySet()
        {
            _view.Data.Select(c => c.Id).Should().BeEquivalentTo(_expected.Select(c => c.Id));
        }

        [Test]
        public void ShouldHaveSavedNewCategorySet()
        {
            CategoryTestRepository.GetCategoriesByBook(_book.Id).Select(c => c.Id)
                .Should().BeEquivalentTo(_expected.Select(c => c.Id));
        }

        [Test]
        public void ShouldHaveRemovedCategoryNotInNewSet()
        {
            CategoryTestRepository.GetCategoriesByBook(_book.Id).Select(c => c.Id)
                .Should().NotContain(_removed.Id);
        }
    }
}
