using System.Net.Http;
using System.Threading.Tasks;
using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.Helpers;
using Inshapardaz.Domain.Models;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.Series.UploadSeriesImage
{
    [TestFixture]
    public class WhenUploadingSeriesImageWhenNoExistingImage : TestBase
    {
        private HttpResponseMessage _response;
        private int _seriesId;

        public WhenUploadingSeriesImageWhenNoExistingImage()
            : base(Role.Writer)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var series = SeriesBuilder.WithLibrary(LibraryId).WithoutImage().Build();
            _seriesId = series.Id;

            _response = await Client.PutFile($"/libraries/{LibraryId}/series/{series.Id}/image", RandomData.Bytes);
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
            var seriesAssert = SeriesAssert.WithResponse(_response).InLibrary(LibraryId);
            seriesAssert.ShouldHaveCorrectImageLocationHeader(_seriesId);
        }

        [Test]
        public void ShouldHaveAddedImageToSeries()
        {
            SeriesAssert.ShouldHaveAddedSeriesImage(_seriesId, DatabaseConnection, FileStore);
        }

        [Test]
        public void ShouldSavePublicImage()
        {
            SeriesAssert.ShouldHavePublicImage(_seriesId, DatabaseConnection);
        }
    }
}
