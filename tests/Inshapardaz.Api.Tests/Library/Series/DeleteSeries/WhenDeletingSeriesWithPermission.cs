using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using FluentAssertions;
using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.DataHelpers;
using Inshapardaz.Api.Tests.Dto;
using Inshapardaz.Api.Tests.Helpers;
using Inshapardaz.Domain.Models;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.Series.DeleteSeries
{
    [TestFixture(Role.Admin)]
    [TestFixture(Role.LibraryAdmin)]
    [TestFixture(Role.Writer)]
    public class WhenDeletingSeriesWithPermission : TestBase
    {
        private HttpResponseMessage _response;

        private SeriesDto _expected;

        public WhenDeletingSeriesWithPermission(Role role)
            : base(role)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var series = SeriesBuilder.WithLibrary(LibraryId).WithBooks(3).Build(4);
            _expected = series.PickRandom();

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
            SeriesAssert.ShouldHaveDeletedSeriesImage(_expected.Id, DatabaseConnection);
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
