using FluentAssertions;
using Inshapardaz.Domain.Repositories;
using Inshapardaz.Functions.Tests.DataHelpers;
using Inshapardaz.Functions.Tests.Dto;
using Inshapardaz.Functions.Tests.Helpers;
using Inshapardaz.Functions.Views.Library;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Threading;

namespace Inshapardaz.Functions.Tests.Asserts
{
    internal class SeriesAssert
    {
        private SeriesView _series;
        private int _libraryId;

        public ObjectResult _response;

        private SeriesAssert(ObjectResult response)
        {
            _response = response;
            _series = response.Value as SeriesView;
        }

        private SeriesAssert(SeriesView view)
        {
            _series = view;
        }

        public static SeriesAssert WithResponse(ObjectResult response)
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
                  .EndingWith($"library/{_libraryId}/series/{_series.Id}/books");

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

            return this;
        }

        internal SeriesAssert WithEditableLinks()
        {
            ShouldHaveSelfLink();
            ShouldHaveBooksLink();
            ShouldHaveUpdateLink();
            ShouldHaveDeleteLink();

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
            var response = _response as CreatedResult;
            response.Location.Should().NotBeNull();
            response.Location.Should().EndWith($"library/{_libraryId}/series/{_series.Id}");
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
    }
}
