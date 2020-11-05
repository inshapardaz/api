using System.Net.Http;
using System.Threading.Tasks;
using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.Helpers;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.Series.UploadSeriesImage
{
    [TestFixture]
    public class WhenUploadingSeriesImageAsReader : TestBase
    {
        private HttpResponseMessage _response;
        private int _seriesId;
        private byte[] _newImage;

        public WhenUploadingSeriesImageAsReader()
            : base(Domain.Adapters.Permission.Reader)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var series = SeriesBuilder.WithLibrary(LibraryId).Build();
            _seriesId = series.Id;
            _newImage = Random.Bytes;

            _response = await Client.PutFile($"/library/{LibraryId}/series/{series.Id}/image", _newImage);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldHaveForbidResult()
        {
            _response.ShouldBeForbidden();
        }

        [Test]
        public void ShouldNotHaveUpdatedSeriesImage()
        {
            SeriesAssert.ShouldNotHaveUpdatedSeriesImage(_seriesId, _newImage, DatabaseConnection, FileStore);
        }
    }
}
