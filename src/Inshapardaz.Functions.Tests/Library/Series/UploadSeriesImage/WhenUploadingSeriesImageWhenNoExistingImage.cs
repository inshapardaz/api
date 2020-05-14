using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Repositories;
using Inshapardaz.Functions.Tests.Asserts;
using Inshapardaz.Functions.Tests.DataBuilders;
using Inshapardaz.Functions.Tests.Fakes;
using Inshapardaz.Functions.Tests.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Functions.Tests.Library.Series.UploadSeriesImage
{
    [TestFixture]
    public class WhenUploadingSeriesImageWhenNoExistingImage : LibraryTest<Functions.Library.Series.UpdateSeriesImage>
    {
        private CreatedResult _response;
        private SeriesDataBuilder _builder;
        private FakeFileStorage _fileStorage;
        private int _seriesId;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _builder = Container.GetService<SeriesDataBuilder>();
            _fileStorage = Container.GetService<IFileStorage>() as FakeFileStorage;

            var series = _builder.WithLibrary(LibraryId).WithoutImage().Build();
            _seriesId = series.Id;

            var request = new RequestBuilder().WithImage().BuildRequestMessage();
            _response = (CreatedResult)await handler.Run(request, LibraryId, series.Id, AuthenticationBuilder.WriterClaim, CancellationToken.None);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            _builder.CleanUp();
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
            SeriesAssert.ShouldHaveAddedSeriesImage(_seriesId, DatabaseConnection, _fileStorage);
        }

        [Test]
        public void ShouldSavePublicImage()
        {
            SeriesAssert.ShouldHavePublicImage(_seriesId, DatabaseConnection);
        }
    }
}
