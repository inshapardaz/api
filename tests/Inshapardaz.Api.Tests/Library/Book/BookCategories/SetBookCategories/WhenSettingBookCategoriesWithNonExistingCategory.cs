using FluentAssertions;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Domain.Models;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.Book.BookCategories.SetBookCategories
{
    public class WhenSettingBookCategoriesWithNonExistingCategory() : TestBase(Role.Writer)
    {
        private HttpResponseMessage _response;
        private BookDto _book;
        private List<CategoryDto> _existing;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _existing = CategoryBuilder.WithLibrary(LibraryId).Build(2).ToList();
            _book = BookBuilder.WithLibrary(LibraryId).WithCategories(_existing).Build();

            _response = await Client.PutObject($"/libraries/{LibraryId}/books/{_book.Id}/categories", new List<int> { _existing[0].Id, -RandomData.Number });
        }

        [OneTimeTearDown]
        public void Teardown() => Cleanup();

        [Test]
        public void ShouldReturnBadRequest() => _response.ShouldBeBadRequest();

        [Test]
        public void ShouldNotHaveChangedExistingCategories()
        {
            CategoryTestRepository.GetCategoriesByBook(_book.Id).Select(c => c.Id)
                .Should().BeEquivalentTo(_existing.Select(c => c.Id));
        }
    }
}
