using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.DataHelpers;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Domain.Models;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.BookShelf.DeleteBookShelf
{
    [TestFixture(Role.Admin)]
    [TestFixture(Role.LibraryAdmin)]
    [TestFixture(Role.Writer)]
    public class WhenDeletingSeriesWithPermission : TestBase
    {
        private HttpResponseMessage _response;

        private SeriesDto _expected;
        private string _filePath;

        public WhenDeletingSeriesWithPermission(Role role)
            : base(role)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var series = SeriesBuilder.WithLibrary(LibraryId).WithBooks(3).Build(4);
            _expected = series.PickRandom();
            _filePath = DatabaseConnection.GetFileById(_expected.ImageId.Value)?.FilePath;

            _response = await Client.DeleteAsync($"/libraries/{LibraryId}/series/{_expected.Id}");
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldReturnNoContent()
        {
            _response.ShouldBeNoContent();
        }

        [Test]
        public void ShouldHaveDeletedSeries()
        {
            SeriesAssert.ShouldHaveDeletedSeries(_expected.Id, DatabaseConnection);
        }

        [Test]
        public void ShouldHaveDeletedTheSeriesImage()
        {
            SeriesAssert.ShouldHaveDeletedSeriesImage(_expected.Id, _expected.ImageId.Value, _filePath, DatabaseConnection, FileStore);
        }

        [Test]
        public void ShouldNotDeleteSeriesBooks()
        {
            var seriesBooks = SeriesBuilder.Books.Where(b => b.SeriesId == _expected.Id);
            foreach (var book in seriesBooks)
            {
                var b = DatabaseConnection.GetBookById(book.Id);
                b.SeriesId.Should().BeNull();
                b.SeriesIndex.Should().BeNull();
            }
        }
    }
}
