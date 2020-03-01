using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using Bogus;
using Inshapardaz.Functions.Tests.DataBuilders;
using Inshapardaz.Functions.Tests.Helpers;
using Inshapardaz.Functions.Views.Library;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Functions.Tests.Library.Series.UpdateSeries
{
    [TestFixture]
    public class WhenUpdatingSeriesAsUnauthorized : LibraryTest
    {
        private UnauthorizedResult _response;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var handler = Container.GetService<Functions.Library.Series.UpdateSeries>();
            var series = new Fixture().Build<SeriesView>().Without(s => s.Links).Without(s => s.BookCount).Create();
            var request = new RequestBuilder()
                                            .WithJsonBody(series)
                                            .Build();
            _response = (UnauthorizedResult)await handler.Run(request, LibraryId, series.Id, AuthenticationBuilder.Unauthorized, CancellationToken.None);
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
