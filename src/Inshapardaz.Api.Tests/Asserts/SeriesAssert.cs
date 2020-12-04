using FluentAssertions;
using Inshapardaz.Api.Tests.DataHelpers;
using Inshapardaz.Api.Tests.Dto;
using Inshapardaz.Api.Tests.Helpers;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Repositories;
using System.Data;
using System.Net.Http;
using System.Threading;

namespace Inshapardaz.Api.Tests.Asserts
{
    internal class SeriesAssert
    {
        private SeriesView _series;
        private int _libraryId;

        public HttpResponseMessage _response;

        private SeriesAssert(HttpResponseMessage response)
        {
            _response = response;
            _series = response.GetContent<SeriesView>().Result;
        }

        private SeriesAssert(SeriesView view)
        {
            _series = view;
        }

        public static SeriesAssert WithResponse(HttpResponseMessage response)
        {
            return new SeriesAssert(response);
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
                  .EndingWith($"library/{_libraryId}/series/{_series.Id}");

            return this;
        }

        public SeriesAssert ShouldHaveBooksLink()
        {
            _series.Link("books")
                  .ShouldBeGet()
                  .EndingWith($"library/{_libraryId}/books")
                  .ShouldHaveQueryParameter("seriesid", _series.Id);

            return this;
        }

        public SeriesAssert ShouldHaveUpdateLink()
        {
            _series.UpdateLink()
                  .ShouldBePut()
                  .EndingWith($"library/{_libraryId}/series/{_series.Id}");

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
                  .EndingWith($"library/{_libraryId}/series/{_series.Id}");
            return this;
        }

        internal SeriesAssert ShouldHaveCorrectImageLocationHeader(int seriesId)
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
                   .EndingWith($"library/{_libraryId}/series/{_series.Id}/image");
            return this;
        }

        internal SeriesAssert ShouldHavePublicImageLink()
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
                  .EndingWith($"library/{_libraryId}/series/{_series.Id}/image");

            return this;
        }

        public SeriesAssert ShouldNotHaveImageUploadLink()
        {
            _series.Link("image-upload").Should().BeNull();

            return this;
        }

        internal SeriesAssert WithBookCount(int count)
        {
            _series.BookCount.Should().Be(count);
            return this;
        }

        internal SeriesAssert ShouldBeSameAs(SeriesDto expected)
        {
            _series.Should().NotBeNull();
            _series.Name.Should().Be(expected.Name);
            return this;
        }

        internal SeriesAssert WithReadOnlyLinks()
        {
            ShouldHaveSelfLink();
            ShouldHaveBooksLink();
            ShouldNotHaveUpdateLink();
            ShouldNotHaveDeleteLink();
            ShouldNotHaveImageUpdateLink();

            return this;
        }

        internal SeriesAssert WithEditableLinks()
        {
            ShouldHaveSelfLink();
            ShouldHaveBooksLink();
            ShouldHaveUpdateLink();
            ShouldHaveDeleteLink();
            ShouldHaveImageUpdateLink();

            return this;
        }

        internal static SeriesAssert FromObject(SeriesView series)
        {
            return new SeriesAssert(series);
        }

        internal static void ShouldHaveDeletedSeries(int seriesId, IDbConnection dbConnection)
        {
            var series = dbConnection.GetSeriesById(seriesId);
            series.Should().BeNull();
        }

        internal static void ShouldNotHaveDeletedSeries(int seriesId, IDbConnection dbConnection)
        {
            var series = dbConnection.GetSeriesById(seriesId);
            series.Should().NotBeNull();
        }

        public SeriesAssert ShouldNotHaveDeleteLink()
        {
            _series.DeleteLink().Should().BeNull();

            return this;
        }

        internal SeriesAssert ShouldHaveCorrectLocationHeader()
        {
            _response.Headers.Location.Should().NotBeNull();
            _response.Headers.Location.AbsolutePath.Should().EndWith($"library/{_libraryId}/series/{_series.Id}");
            return this;
        }

        public SeriesAssert ShouldHaveSavedSeries(IDbConnection dbConnection)
        {
            var dbSeries = dbConnection.GetSeriesById(_series.Id);
            dbSeries.Should().NotBeNull();
            _series.Name.Should().Be(dbSeries.Name);
            return this;
        }

        public SeriesAssert ShouldHaveCorrectSeriesRetunred(SeriesDto series, IDbConnection dbConnection)
        {
            _series.Should().NotBeNull();
            _series.Id.Should().Be(series.Id);
            _series.Name.Should().Be(series.Name);
            _series.BookCount.Should().Be(dbConnection.GetBookCountBySeries(_series.Id));
            return this;
        }

        internal static void ShouldHaveUpdatedSeriesImage(int seriesId, byte[] newImage, IDbConnection dbConnection, IFileStorage fileStorage)
        {
            var imageUrl = dbConnection.GetSeriesImageUrl(seriesId);
            imageUrl.Should().NotBeNull();
            var image = fileStorage.GetFile(imageUrl, CancellationToken.None).Result;
            image.Should().NotBeNull().And.Equal(newImage);
        }

        internal static void ShouldHavePublicImage(int seriesId, IDbConnection dbConnection)
        {
            var image = dbConnection.GetSeriesImage(seriesId);
            image.Should().NotBeNull();
            image.IsPublic.Should().BeTrue();
        }

        internal static void ShouldNotHaveUpdatedSeriesImage(int seriesId, byte[] newImage, IDbConnection dbConnection, IFileStorage fileStorage)
        {
            var imageUrl = dbConnection.GetSeriesImageUrl(seriesId);
            imageUrl.Should().NotBeNull();
            var image = fileStorage.GetFile(imageUrl, CancellationToken.None).Result;
            image.Should().NotEqual(newImage);
        }

        internal static void ShouldHaveAddedSeriesImage(int seriesId, IDbConnection dbConnection, IFileStorage fileStorage)
        {
            var imageUrl = dbConnection.GetSeriesImageUrl(seriesId);
            imageUrl.Should().NotBeNull();
            var image = fileStorage.GetFile(imageUrl, CancellationToken.None).Result;
            image.Should().NotBeNullOrEmpty();
        }

        internal static void ShouldHaveDeletedSeriesImage(int seriesId, IDbConnection dbConnection)
        {
            var image = dbConnection.GetSeriesImage(seriesId);
            image.Should().BeNull();
        }
    }

    internal static class SeriesAssertionExtensions
    {
        internal static SeriesAssert ShouldMatch(this SeriesView view, SeriesDto dto)
        {
            return SeriesAssert.FromObject(view)
                               .ShouldBeSameAs(dto);
        }
    }
}
