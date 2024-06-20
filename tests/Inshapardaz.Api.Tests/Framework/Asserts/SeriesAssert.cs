using FluentAssertions;
using Inshapardaz.Api.Tests.Framework.DataHelpers;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Fakes;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Api.Views.Library;
using System.Net.Http;
using System.Threading;

namespace Inshapardaz.Api.Tests.Framework.Asserts
{
    public class SeriesAssert
    {
        private SeriesView _series;
        private int _libraryId;

        public HttpResponseMessage _response;
        private readonly ISeriesTestRepository _seriesRepository;
        private readonly IFileTestRepository _fileRepository;
        private readonly IAuthorTestRepository _authorRepository;
        private readonly ICategoryTestRepository _categoryRepository;
        private readonly FakeFileStorage _fileStorage;

        public SeriesAssert(ISeriesTestRepository seriesRepository,
            IFileTestRepository fileRepository,
            IAuthorTestRepository authorRepository,
            ICategoryTestRepository categoryRepository,
            FakeFileStorage fileStorage)
        {
            _fileRepository = fileRepository;
            _authorRepository = authorRepository;
            _categoryRepository = categoryRepository;
            _fileStorage = fileStorage;
            _seriesRepository = seriesRepository;
        }

        public SeriesAssert ForResponse(HttpResponseMessage response)
        {
            _response = response;
            _series = response.GetContent<SeriesView>().Result;
            return this;
        }

        public SeriesAssert ForView(SeriesView view)
        {
            _series = view;
            return this;
        }

        public SeriesAssert InLibrary(int libraryId)
        {
            _libraryId = libraryId;
            return this;
        }

        public SeriesAssert ShouldHaveSelfLink()
        {
            _series.SelfLink()
                  .ShouldBeGet()
                  .EndingWith($"libraries/{_libraryId}/series/{_series.Id}");

            return this;
        }

        public SeriesAssert ShouldHaveBooksLink()
        {
            _series.Link("books")
                  .ShouldBeGet()
                  .EndingWith($"libraries/{_libraryId}/books")
                  .ShouldHaveQueryParameter("seriesid", _series.Id);

            return this;
        }

        public SeriesAssert ShouldHaveUpdateLink()
        {
            _series.UpdateLink()
                  .ShouldBePut()
                  .EndingWith($"libraries/{_libraryId}/series/{_series.Id}");

            return this;
        }

        public SeriesAssert ShouldNotHaveUpdateLink()
        {
            _series.UpdateLink().Should().BeNull();
            return this;
        }

        public SeriesAssert ShouldHaveDeleteLink()
        {
            _series.DeleteLink()
                  .ShouldBeDelete()
                  .EndingWith($"libraries/{_libraryId}/series/{_series.Id}");
            return this;
        }

        public SeriesAssert ShouldHaveCorrectImageLocationHeader(int seriesId)
        {
            _response.Headers.Location.Should().NotBeNull();
            _response.Headers.Location.AbsolutePath.Should().Contain($"/files/");
            return this;
        }

        public SeriesAssert ShouldNotHaveImageUpdateLink()
        {
            _series.Link("image-upload").Should().BeNull();
            return this;
        }

        public SeriesAssert ShouldNotHaveImageLink()
        {
            _series.Link("image").Should().BeNull();
            return this;
        }

        public SeriesAssert ShouldHaveImageUpdateLink()
        {
            _series.Link("image-upload")
                   .ShouldBePut()
                   .EndingWith($"libraries/{_libraryId}/series/{_series.Id}/image");
            return this;
        }

        public SeriesAssert ShouldHavePublicImageLink()
        {
            _series.Link("image")
                .ShouldBeGet();
            //.Href.Should().StartWith(Settings.CDNAddress);
            return this;
        }

        public SeriesAssert ShouldHaveImageUploadLink()
        {
            _series.Link("image-upload")
                  .ShouldBePut()
                  .EndingWith($"libraries/{_libraryId}/series/{_series.Id}/image");

            return this;
        }

        public SeriesAssert ShouldNotHaveImageUploadLink()
        {
            _series.Link("image-upload").Should().BeNull();

            return this;
        }

        public SeriesAssert WithBookCount(int count)
        {
            _series.BookCount.Should().Be(count);
            return this;
        }

        public SeriesAssert ShouldBeSameAs(SeriesDto expected)
        {
            _series.Should().NotBeNull();
            _series.Name.Should().Be(expected.Name);
            return this;
        }

        public SeriesAssert WithReadOnlyLinks()
        {
            ShouldHaveSelfLink();
            ShouldHaveBooksLink();
            ShouldNotHaveUpdateLink();
            ShouldNotHaveDeleteLink();
            ShouldNotHaveImageUpdateLink();

            return this;
        }

        public SeriesAssert WithEditableLinks()
        {
            ShouldHaveSelfLink();
            ShouldHaveBooksLink();
            ShouldHaveUpdateLink();
            ShouldHaveDeleteLink();
            ShouldHaveImageUpdateLink();

            return this;
        }

        public SeriesAssert ShouldHaveDeletedSeries(int seriesId)
        {
            var series = _seriesRepository.GetSeriesById(seriesId);
            series.Should().BeNull();
            return this;
        }

        public SeriesAssert ShouldNotHaveDeletedSeries(int seriesId)
        {
            var series = _seriesRepository.GetSeriesById(seriesId);
            series.Should().NotBeNull();
            return this;
        }

        public SeriesAssert ShouldNotHaveDeleteLink()
        {
            _series.DeleteLink().Should().BeNull();

            return this;
        }

        public SeriesAssert ShouldHaveCorrectLocationHeader()
        {
            _response.Headers.Location.Should().NotBeNull();
            _response.Headers.Location.AbsolutePath.Should().EndWith($"libraries/{_libraryId}/series/{_series.Id}");
            return this;
        }

        public SeriesAssert ShouldHaveSavedSeries()
        {
            var dbSeries = _seriesRepository.GetSeriesById(_series.Id);
            dbSeries.Should().NotBeNull();
            _series.Name.Should().Be(dbSeries.Name);
            return this;
        }

        public SeriesAssert ShouldHaveCorrectSeriesRetunred(SeriesDto series)
        {
            _series.Should().NotBeNull();
            _series.Id.Should().Be(series.Id);
            _series.Name.Should().Be(series.Name);
            _series.BookCount.Should().Be(_seriesRepository.GetBookCountBySeries(_series.Id));
            return this;
        }

        public SeriesAssert ShouldHaveUpdatedSeriesImage(int seriesId, byte[] newImage)
        {
            var imageUrl = _seriesRepository.GetSeriesImageUrl(seriesId);
            imageUrl.Should().NotBeNull();
            var image = _fileStorage.GetFile(imageUrl, CancellationToken.None).Result;
            image.Should().NotBeNull().And.Equal(newImage);
            return this;
        }

        public SeriesAssert ShouldHavePublicImage(int seriesId)
        {
            var image = _seriesRepository.GetSeriesImage(seriesId);
            image.Should().NotBeNull();
            image.IsPublic.Should().BeTrue();
            return this;
        }

        public SeriesAssert ShouldNotHaveUpdatedSeriesImage(int seriesId, byte[] newImage)
        {
            var imageUrl = _seriesRepository.GetSeriesImageUrl(seriesId);
            imageUrl.Should().NotBeNull();
            var image = _fileStorage.GetFile(imageUrl, CancellationToken.None).Result;
            image.Should().NotEqual(newImage);
            return this;
        }

        public SeriesAssert ShouldHaveAddedSeriesImage(int seriesId)
        {
            var imageUrl = _seriesRepository.GetSeriesImageUrl(seriesId);
            imageUrl.Should().NotBeNull();
            var image = _fileStorage.GetFile(imageUrl, CancellationToken.None).Result;
            image.Should().NotBeNullOrEmpty();
            return this;
        }

        public SeriesAssert ShouldHaveDeletedSeriesImage(int seriesId, long imageId, string filePath)
        {
            var image = _seriesRepository.GetSeriesImage(seriesId);
            image.Should().BeNull();
            var file = _fileRepository.GetFileById(imageId);
            file.Should().BeNull();
            _fileStorage.DoesFileExists(filePath).Should().BeFalse();
            return this;
        }
    }
}
