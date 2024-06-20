using System.Net.Http;
using System.Threading.Tasks;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.Series.UploadSeriesImage
{
    [TestFixture(Role.Admin)]
    [TestFixture(Role.LibraryAdmin)]
    [TestFixture(Role.Writer)]
    public class WhenUploadingSeriesImageWithPermissions : TestBase
    {
        private HttpResponseMessage _response;
        private SeriesAssert _assert;
        private int _seriesId;
        private byte[] _newImage;

        public WhenUploadingSeriesImageWithPermissions(Role role)
            : base(role)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var series = SeriesBuilder.WithLibrary(LibraryId).Build();
            _seriesId = series.Id;
            _newImage = RandomData.Bytes;
            _response = await Client.PutFile($"/libraries/{LibraryId}/series/{series.Id}/image", _newImage);
            _assert = Services.GetService<SeriesAssert>().ForResponse(_response).InLibrary(LibraryId);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldReturnOk()
        {
            _response.ShouldBeOk();
        }

        [Test]
        public void ShouldHaveUpdatedSeriesImage()
        {
            _assert.ShouldHaveUpdatedSeriesImage(_seriesId, _newImage);
        }
    }
}
