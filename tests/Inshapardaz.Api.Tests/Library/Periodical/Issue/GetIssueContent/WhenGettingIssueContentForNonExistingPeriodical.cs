using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.Helpers;
using NUnit.Framework;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.Periodical.Issue.GetIssueContent
{
    [TestFixture]
    public class WhenGettingIssueContentForNonExistingPeriodical
        : TestBase
    {
        private HttpResponseMessage _response;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var issue = IssueBuilder.WithLibrary(LibraryId).WithContent().Build();
            var content = IssueBuilder.Contents.First();
            _response = await Client.GetAsync($"/libraries/{LibraryId}/periodicals/{-RandomData.Number}/volumes/{issue.VolumeNumber}/issues/{issue.IssueNumber}/contents?language={content.Language}", content.MimeType);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldHaveNotFoundResult()
        {
            _response.ShouldBeNotFound();
        }
    }
}
