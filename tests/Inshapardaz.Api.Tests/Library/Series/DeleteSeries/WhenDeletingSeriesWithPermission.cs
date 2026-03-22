using FluentAssertions;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.Series.DeleteSeries
{
    [TestFixture(Role.Admin)]
    [TestFixture(Role.LibraryAdmin)]
    [TestFixture(Role.Writer)]
    public class WhenDeletingSeriesWithPermission(Role role) : TestBase(role)
    {
        private HttpResponseMessage _response;
        private SeriesAssert _assert;
        private SeriesDto _expected;

        private string _filePath;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var series = SeriesBuilder.WithLibrary(LibraryId).WithBooks(3).Build(4);
            _expected = series.PickRandom();
            _filePath = FileTestRepository.GetFileById(_expected.ImageId.Value)?.FilePath;

            _response = await Client.DeleteAsync($"/libraries/{LibraryId}/series/{_expected.Id}");
            _assert = Services.GetService<SeriesAssert>().ForResponse(_response).InLibrary(LibraryId);
        }

        [OneTimeTearDown]
        public void Teardown() => Cleanup();

        [Test]
        public void ShouldReturnNoContent() => _response.ShouldBeNoContent();

        [Test]
        public void ShouldHaveDeletedSeries() => _assert.ShouldHaveDeletedSeries(_expected.Id);

        [Test]
        public void ShouldHaveDeletedTheSeriesImage() => _assert.ShouldHaveDeletedSeriesImage(_expected.Id, _expected.ImageId.Value, _filePath);

        [Test]
        public void ShouldNotDeleteSeriesBooks()
        {
            var seriesBooks = SeriesBuilder.Books.Where(b => b.SeriesId == _expected.Id);
            foreach (var book in seriesBooks)
            {
                var b = BookTestRepository.GetBookById(book.Id);
                b.SeriesId.Should().BeNull();
                b.SeriesIndex.Should().BeNull();
            }
        }
    }
}
