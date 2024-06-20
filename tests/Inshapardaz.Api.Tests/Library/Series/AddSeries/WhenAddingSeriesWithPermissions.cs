using System.Net.Http;
using System.Threading.Tasks;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.Series.AddSeries
{
    [TestFixture(Role.Admin)]
    [TestFixture(Role.LibraryAdmin)]
    [TestFixture(Role.Writer)]
    public class WhenAddingSeriesWithPermissions
        : TestBase
    {
        private SeriesAssert _assert;
        private HttpResponseMessage _response;

        public WhenAddingSeriesWithPermissions(Role role) : base(role)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var series = new SeriesView { Name = RandomData.Name };

            _response = await Client.PostObject($"/libraries/{LibraryId}/series", series);

            _assert = Services.GetService<SeriesAssert>().ForResponse(_response).InLibrary(LibraryId);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
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
            _assert.ShouldHaveCorrectLocationHeader();
        }

        [Test]
        public void ShouldSaveTheSeries()
        {
            _assert.ShouldHaveSavedSeries();
        }

        [Test]
        public void ShouldHaveLinks()
        {
            _assert.ShouldHaveSelfLink()
                         .ShouldHaveBooksLink()
                         .ShouldHaveUpdateLink()
                         .ShouldHaveDeleteLink()
                         .ShouldHaveImageUpdateLink();
        }
    }
}
