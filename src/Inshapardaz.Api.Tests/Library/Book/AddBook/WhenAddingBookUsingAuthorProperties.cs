using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.Helpers;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Models;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.Book.AddBook
{
    [TestFixture]
    public class WhenAddingBookUsingAuthorProperties : TestBase
    {
        private BookAssert _bookAssert;
        private HttpResponseMessage _response;

        public WhenAddingBookUsingAuthorProperties() : base(Role.LibraryAdmin)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var author = AuthorBuilder.WithLibrary(LibraryId).Build();
            var series = SeriesBuilder.WithLibrary(LibraryId).Build();
            var categories = CategoryBuilder.WithLibrary(LibraryId).Build(3);
            var book = new BookView
            {
                Title = RandomData.Name,
                Authors = new List<AuthorView> { new AuthorView { Id = author.Id } },
                SeriesId = series.Id,
                SeriesIndex = 1,
                SeriesName = series.Name,
                Language = RandomData.Locale,
                Categories = RandomData.PickRandom(categories, 2).Select(c => new CategoryView { Id = c.Id })
            };

            _response = await Client.PostObject($"/libraries/{LibraryId}/books", book);
            _bookAssert = BookAssert.WithResponse(_response).InLibrary(LibraryId);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldHaveCreatedResult()
        {
            _response.ShouldBeCreated();
        }

        [Test]
        public void ShouldHaveLocationHeader()
        {
            _bookAssert.ShouldHaveCorrectLocationHeader();
        }

        [Test]
        public void ShouldSaveTheBook()
        {
            _bookAssert.ShouldHaveSavedBook(DatabaseConnection);
        }

        [Test]
        public void ShouldHaveLinks()
        {
            _bookAssert.ShouldHaveSelfLink()
                        .ShouldHaveUpdateLink()
                        .ShouldHaveDeleteLink()
                        .ShouldHaveImageUpdateLink();
        }
    }
}
