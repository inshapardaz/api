using System.Net.Http;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.Helpers;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Adapters;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.Series.AddSeries
{
    [TestFixture(Permission.Admin)]
    [TestFixture(Permission.LibraryAdmin)]
    [TestFixture(Permission.Writer)]
    public class WhenAddingSeriesWithPermissions
        : TestBase
    {
        private readonly ClaimsPrincipal _claim;
        private SeriesAssert _assert;
        private HttpResponseMessage _response;

        public WhenAddingSeriesWithPermissions(Permission permission) : base(permission)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var series = new SeriesView { Name = Random.Name };

            _response = await Client.PostObject($"/library/{LibraryId}/series", series);

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
