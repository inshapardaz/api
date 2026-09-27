using FluentAssertions;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Domain.Models;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.Book.BookCategories.SetBookCategories
{
    public class WhenSettingBookCategoriesToEmpty() : TestBase(Role.Writer)
    {
        private HttpResponseMessage _response;
        private BookDto _book;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var categories = CategoryBuilder.WithLibrary(LibraryId).Build(3).ToList();
            _book = BookBuilder.WithLibrary(LibraryId).WithCategories(categories).Build();

            _response = await Client.PutObject($"/libraries/{LibraryId}/books/{_book.Id}/categories", new List<int>());
        }

        [OneTimeTearDown]
        public void Teardown() => Cleanup();

        [Test]
        public void ShouldReturnOk() => _response.ShouldBeOk();

        [Test]
        public void ShouldHaveRemovedAllCategories() => CategoryTestRepository.GetCategoriesByBook(_book.Id).Should().BeEmpty();
    }
}
