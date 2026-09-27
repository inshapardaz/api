using FluentAssertions;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Domain.Models;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.Book.BookCategories.SetBookCategories
{
    public class WhenSettingBookCategoriesWithParentAndChild() : TestBase(Role.Writer)
    {
        private HttpResponseMessage _response;
        private BookDto _book;
        private CategoryDto _parent;
        private CategoryDto _child;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _parent = CategoryBuilder.WithLibrary(LibraryId).Build();
            _child = CategoryBuilder.WithParent(_parent).Build();
            _book = BookBuilder.WithLibrary(LibraryId).Build();

            _response = await Client.PutObject($"/libraries/{LibraryId}/books/{_book.Id}/categories", new List<int> { _parent.Id, _child.Id });
        }

        [OneTimeTearDown]
        public void Teardown() => Cleanup();

        [Test]
        public void ShouldReturnOk() => _response.ShouldBeOk();

        [Test]
        public void ShouldHaveSavedBothCategories()
        {
            CategoryTestRepository.GetCategoriesByBook(_book.Id).Select(c => c.Id)
                .Should().BeEquivalentTo(new[] { _parent.Id, _child.Id });
        }
    }
}
