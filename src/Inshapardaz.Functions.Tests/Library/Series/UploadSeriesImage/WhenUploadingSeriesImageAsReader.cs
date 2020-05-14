using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Repositories;
using Inshapardaz.Functions.Tests.Asserts;
using Inshapardaz.Functions.Tests.DataBuilders;
using Inshapardaz.Functions.Tests.DataHelpers;
using Inshapardaz.Functions.Tests.Fakes;
using Inshapardaz.Functions.Tests.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Functions.Tests.Library.Series.UploadSeriesImage
{
    [TestFixture]
    public class WhenUploadingSeriesImageAsReader : LibraryTest<Functions.Library.Series.UpdateSeriesImage>
    {
        private ForbidResult _response;
        private SeriesDataBuilder _builder;
        private FakeFileStorage _fileStorage;
        private int _seriesId;
        private byte[] _oldImage;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _builder = Container.GetService<SeriesDataBuilder>();
            _fileStorage = Container.GetService<IFileStorage>() as FakeFileStorage;

            var series = _builder.WithLibrary(LibraryId).Build();
            _seriesId = series.Id;
            var imageUrl = DatabaseConnection.GetSeriesImageUrl(_seriesId);
            _oldImage = await _fileStorage.GetFile(imageUrl, CancellationToken.None);
            var request = new RequestBuilder().WithImage().BuildRequestMessage();
            _response = (ForbidResult)await handler.Run(request, LibraryId, _seriesId, AuthenticationBuilder.ReaderClaim, CancellationToken.None);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            _builder.CleanUp();
            Cleanup();
        }

        [Test]
        public void ShouldHaveForbidResult()
        {
            Assert.That(_response, Is.Not.Null);
        }

        [Test]
        public void ShouldNotHaveUpdatedSeriesImage()
        {
            SeriesAssert.ShouldNotHaveUpdatedSeriesImage(_seriesId, _oldImage, DatabaseConnection, _fileStorage);
        }
    }
}
