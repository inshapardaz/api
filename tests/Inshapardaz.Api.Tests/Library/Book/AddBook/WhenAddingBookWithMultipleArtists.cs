using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.Book.AddBook
{
    [TestFixture(Role.LibraryAdmin)]
    public class WhenAddingBookWithMultipleArtists : TestBase
    {
        private BookAssert _bookAssert;
        private HttpResponseMessage _response;

        public WhenAddingBookWithMultipleArtists(Role role) : base(role)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var authors = AuthorBuilder.WithLibrary(LibraryId).Build(3);
            var series = SeriesBuilder.WithLibrary(LibraryId).Build();
            var categories = CategoryBuilder.WithLibrary(LibraryId).Build(3);
            var book = new BookView
            {
                Title = RandomData.Name,
                SeriesId = series.Id,
                SeriesIndex = 1,
                SeriesName = series.Name,
                Language = RandomData.Locale,
                Authors = RandomData.PickRandom(authors, 2).Select(c => new AuthorView { Id = c.Id }),
                Categories = RandomData.PickRandom(categories, 2).Select(c => new CategoryView { Id = c.Id })
            };

            _response = await Client.PostObject($"/libraries/{LibraryId}/books", book);
            _bookAssert = Services.GetService<BookAssert>().ForResponse(_response).ForLibrary(LibraryId);
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
            _bookAssert.ShouldHaveSavedBook();
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
