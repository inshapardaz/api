using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.Helpers;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Adapters;
using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.Periodical.AddPeriodical
{
    [TestFixture(Permission.Admin)]
    [TestFixture(Permission.LibraryAdmin)]
    [TestFixture(Permission.Writer)]
    public class WhenAddingPeriodicalWithPermissions
        : TestBase
    {
        private PeriodicalAssert _assert;
        private HttpResponseMessage _response;

        public WhenAddingPeriodicalWithPermissions(Permission permission)
            : base(permission, true)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var periodical = new PeriodicalView { Title = Random.Name, Description = Random.Words(20) };

            _response = await Client.PostObject($"/library/{LibraryId}/periodicals", periodical);

            _assert = PeriodicalAssert.WithResponse(_response).InLibrary(LibraryId);
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
        public void ShouldSaveThePeriodical()
        {
            _assert.ShouldHaveSavedPeriodical(DatabaseConnection);
        }

        [Test]
        public void ShouldHaveLinks()
        {
            _assert.ShouldHaveSelfLink()
                         .ShouldHaveIssuesLink()
                         .ShouldHaveUpdateLink()
                         .ShouldHaveDeleteLink()
                         .ShouldHaveImageUpdateLink();
        }
    }
}
