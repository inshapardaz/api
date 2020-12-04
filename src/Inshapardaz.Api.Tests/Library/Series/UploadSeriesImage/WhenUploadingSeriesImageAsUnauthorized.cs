using System.Net.Http;
using System.Threading.Tasks;
using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.Helpers;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.Series.UploadSeriesImage
{
    [TestFixture]
    public class WhenUploadingSeriesImageAsUnauthorized : TestBase
    {
        private HttpResponseMessage _response;
        private int _seriesId;
        private byte[] _newImage;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var series = SeriesBuilder.WithLibrary(LibraryId).Build();
            _seriesId = series.Id;
            _newImage = Random.Bytes;

            _response = await Client.PutFile($"/libraries/{LibraryId}/series/{_seriesId}/image", _newImage);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldHaveUnauthorizedResult()
        {
            _response.ShouldBeUnauthorized();
        }

        [Test]
        public void ShouldNotHaveUpdatedSeriesImage()
        {
            SeriesAssert.ShouldNotHaveUpdatedSeriesImage(_seriesId, _newImage, DatabaseConnection, FileStore);
        }
    }
}