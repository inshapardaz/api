using System.Net.Http;
using System.Threading.Tasks;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.Series.UploadSeriesImage
{
    [TestFixture]
    public class WhenUploadingSeriesImageAsUnauthorized : TestBase
    {
        private HttpResponseMessage _response;
        private SeriesAssert _assert;
        private int _seriesId;
        private byte[] _newImage;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var series = SeriesBuilder.WithLibrary(LibraryId).Build();
            _seriesId = series.Id;
            _newImage = RandomData.Bytes;

            _response = await Client.PutFile($"/libraries/{LibraryId}/series/{_seriesId}/image", _newImage);
            _assert = Services.GetService<SeriesAssert>().ForResponse(_response).InLibrary(LibraryId);
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
            _assert.ShouldNotHaveUpdatedSeriesImage(_seriesId, _newImage);
        }
    }
}
