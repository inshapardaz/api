using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using Inshapardaz.Functions.Tests.DataBuilders;
using Inshapardaz.Functions.Tests.Helpers;
using Inshapardaz.Functions.Views.Library;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Functions.Tests.Library.Series.UpdateSeries
{
    [TestFixture]
    public class WhenUpdatingSeriesAsReader : LibraryTest
    {
        private ForbidResult _response;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var handler = Container.GetService<Functions.Library.Series.UpdateSeries>();
            var series = new Fixture().Build<SeriesView>().Without(s => s.Links).Without(s => s.BookCount).Create();
            _response = (ForbidResult)await handler.Run(series.ToRequest(), LibraryId, series.Id, AuthenticationBuilder.ReaderClaim, CancellationToken.None);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldHaveForbiddenResult()
        {
            Assert.That(_response, Is.Not.Null);
        }
    }
}
