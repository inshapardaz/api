using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.Helpers;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Models;
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

            _assert = SeriesAssert.WithResponse(_response).InLibrary(LibraryId);
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
            _assert.ShouldHaveSavedSeries(DatabaseConnection);
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
