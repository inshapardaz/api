using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Models;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.Series.AddSeries
{
    [TestFixture]
    public class WhenAddingSeriesAsReader() : TestBase(Role.Reader)
    {
        private HttpResponseMessage _response;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var series = new SeriesView { Name = RandomData.Name };

            _response = await Client.PostObject($"/libraries/{LibraryId}/series", series);
        }

        [OneTimeTearDown]
        public void Teardown() => Cleanup();

        [Test]
        public void ShouldHaveForbiddenResult() => _response.ShouldBeForbidden();
    }
}
