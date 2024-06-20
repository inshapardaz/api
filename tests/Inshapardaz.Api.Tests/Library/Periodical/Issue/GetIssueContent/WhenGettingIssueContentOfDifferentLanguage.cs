using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Domain.Models;
using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.Periodical.Issue.GetIssueContent
{
    [TestFixture]
    public class WhenGettingIssueContentOfDifferentLanguage
        : TestBase
    {
        private HttpResponseMessage _response;

        public WhenGettingIssueContentOfDifferentLanguage()
            : base(Role.Reader)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var issue = IssueBuilder.WithLibrary(LibraryId).WithContents(2).Build();
            var expected = IssueBuilder.Contents.PickRandom();
            _response = await Client.GetAsync($"/libraries/{LibraryId}/periodicals/{issue.PeriodicalId}/volumes/{issue.VolumeNumber}/issues/{issue.IssueNumber}/contents//{expected.Id}?language={expected.Language + "1"}", expected.MimeType);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldReturnNotFound()
        {
            _response.ShouldBeNotFound();
        }
    }
}
