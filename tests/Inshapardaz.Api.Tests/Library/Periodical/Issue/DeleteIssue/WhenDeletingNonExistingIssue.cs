using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Domain.Models;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.Periodical.Issue.DeleteIssue
{
    [TestFixture]
    public class WhenDeletingNonExistingIssue() : TestBase(Role.Writer)
    {
        private HttpResponseMessage _response;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var issue = IssueBuilder.WithLibrary(LibraryId).Build();
            _response = await Client.DeleteAsync($"/libraries/{LibraryId}/periodicals/{issue. PeriodicalId}/volumes/{issue.VolumeNumber}/issues/{-RandomData.Number}");
        }

        [OneTimeTearDown]
        public void Teardown() => Cleanup();

        [Test]
        public void ShouldReturnNoContent() => _response.ShouldBeNoContent();
    }
}
