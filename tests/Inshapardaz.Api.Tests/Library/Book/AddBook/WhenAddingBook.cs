using System.Collections.Generic;
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
    [TestFixture]
    public class WhenAddingBook : TestBase
    {
        private BookAssert _bookAssert;
        private HttpResponseMessage _response;

        public WhenAddingBook() : base(Role.LibraryAdmin)
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
                Source = RandomData.String,
                Publisher = RandomData.String,
                Language = RandomData.Locale,
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
