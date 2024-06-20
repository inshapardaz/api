using System.Net.Http;
using System.Threading.Tasks;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.Series.UploadSeriesImage
{
    [TestFixture]
    public class WhenUploadingSeriesImageWhenNoExistingImage : TestBase
    {
        private HttpResponseMessage _response;
        private SeriesAssert _assert;
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
            _assert = Services.GetService<SeriesAssert>().ForResponse(_response).InLibrary(LibraryId);
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
            _assert.ShouldHaveCorrectImageLocationHeader(_seriesId);
        }

        [Test]
        public void ShouldHaveAddedImageToSeries()
        {
            _assert.ShouldHaveAddedSeriesImage(_seriesId);
        }

        [Test]
        public void ShouldSavePublicImage()
        {
            _assert.ShouldHavePublicImage(_seriesId);
        }
    }
}
