using FluentAssertions;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Domain.Models;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.Book.BookCategories.AddCategoryToBook
{
    public class WhenAddingNonExistingCategoryToBook() : TestBase(Role.Writer)
    {
        private HttpResponseMessage _response;
        private BookDto _book;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _book = BookBuilder.WithLibrary(LibraryId).Build();

            _response = await Client.PostAsync($"/libraries/{LibraryId}/books/{_book.Id}/categories/{-RandomData.Number}", null);
        }

        [OneTimeTearDown]
        public void Teardown() => Cleanup();

        [Test]
        public void ShouldReturnNotFound() => _response.ShouldBeNotFound();

        [Test]
        public void ShouldNotHaveAddedAnyCategory() => CategoryTestRepository.GetCategoriesByBook(_book.Id).Should().BeEmpty();
    }
}
