using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Functions.Tests.Asserts;
using Inshapardaz.Functions.Tests.Helpers;
using Inshapardaz.Functions.Views.Library;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;

namespace Inshapardaz.Functions.Tests.Library.Series.AddSeries
{
    [TestFixture]
    public class WhenAddingSeriesAsReader : LibraryTest<Functions.Library.Series.AddSeries>
    {
        private ForbidResult _response;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var series = new SeriesView { Name = Random.Name };

            _response = (ForbidResult)await handler.Run(series, LibraryId, AuthenticationBuilder.ReaderClaim, CancellationToken.None);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldHaveForbiddenResult()
        {
            _response.ShouldBeForbidden();
        }
    }
}
