using FluentAssertions;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Domain.Models;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.Book.BookCategories.AddCategoryToBook
{
    public class WhenAddingCategoryAlreadyAssignedToBook() : TestBase(Role.Writer)
    {
        private HttpResponseMessage _response;
        private BookDto _book;
        private List<CategoryDto> _existing;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _existing = CategoryBuilder.WithLibrary(LibraryId).Build(2).ToList();
            _book = BookBuilder.WithLibrary(LibraryId).WithCategories(_existing).Build();

            _response = await Client.PostAsync($"/libraries/{LibraryId}/books/{_book.Id}/categories/{_existing[0].Id}", null);
        }

        [OneTimeTearDown]
        public void Teardown() => Cleanup();

        [Test]
        public void ShouldReturnOk() => _response.ShouldBeOk();

        [Test]
        public void ShouldNotHaveDuplicatedTheCategory()
        {
            CategoryTestRepository.GetCategoriesByBook(_book.Id).Select(c => c.Id)
                .Should().BeEquivalentTo(_existing.Select(c => c.Id));
        }
    }
}
