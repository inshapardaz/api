using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Functions.Tests.Asserts;
using Inshapardaz.Functions.Tests.DataBuilders;
using Inshapardaz.Functions.Tests.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Functions.Tests.Library.Series.DeleteSeries
{
    [TestFixture]
    public class WhenDeletingSeriesAsReader : LibraryTest<Functions.Library.Series.DeleteSeries>
    {
        private SeriesDataBuilder _builder;
        private ForbidResult _response;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var request = TestHelpers.CreateGetRequest();
            _builder = Container.GetService<SeriesDataBuilder>();
            var series = _builder.WithLibrary(LibraryId).Build(4);
            var expected = series.First();

            _response = (ForbidResult)await handler.Run(request, LibraryId, expected.Id, AuthenticationBuilder.ReaderClaim, CancellationToken.None);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            _builder.CleanUp();
            Cleanup();
        }

        [Test]
        public void ShouldHaveForbiddenResult()
        {
            _response.ShouldBeForbidden();
        }
    }
}
