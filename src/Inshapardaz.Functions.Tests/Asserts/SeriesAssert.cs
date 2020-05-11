using FluentAssertions;
using Inshapardaz.Functions.Tests.DataHelpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Inshapardaz.Functions.Tests.Asserts
{
    internal class SeriesAssert
    {
        internal static void ShouldNotHaveDeletedSeries(int seriesId, IDbConnection dbConnection)
        {
            var author = dbConnection.GetSeriesById(seriesId);
            author.Should().NotBeNull();
        }

        internal static void ShouldHaveDeletedSeries(int seriesId, IDbConnection dbConnection)
        {
            var author = dbConnection.GetSeriesById(seriesId);
            author.Should().BeNull();
        }
    }
}
