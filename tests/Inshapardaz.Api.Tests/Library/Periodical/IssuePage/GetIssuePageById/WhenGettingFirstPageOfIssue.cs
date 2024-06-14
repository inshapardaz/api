using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Domain.Models;
using NUnit.Framework;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.Periodical.IssuePage.GetIssuePageById
{
    [TestFixture]
    public class WhenGettingFirstPageOfIssue
        : TestBase
    {
        private HttpResponseMessage _response;

        private IssuePageDto _expected;

        private IssuePageAssert _assert;

        public WhenGettingFirstPageOfIssue() : base(Role.Reader)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var issue = IssueBuilder.WithLibrary(LibraryId).WithPages(3).Build();
            _expected = IssueBuilder.GetPages(issue.Id).First();

            _response = await Client.GetAsync($"/libraries/{LibraryId}/periodicals/{issue.PeriodicalId}/volumes/{issue.VolumeNumber}/issues/{issue.IssueNumber}/pages/1");
            _assert = IssuePageAssert.FromResponse(_response, LibraryId);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldHaveOkResult()
        {
            _response.ShouldBeOk();
        }

        [Test]
        public void ShouldHaveCorrectObjectRetured()
        {
            _assert.ShouldMatch(_expected);
        }

        [Test]
        public void ShouldHaveLinks()
        {
            _assert.ShouldHaveSelfLink()
                   .ShouldHavePeriodicalLink()
                   .ShouldHaveIssueLink()
                   .ShouldNotHaveImageLink();
        }

        [Test]
        public void ShouldHaveNextLinks()
        {
            _assert.ShouldHaveNextLinkForPageNumber(2);
        }

        [Test]
        public void ShouldNotHavePreviousLinks()
        {
            _assert.ShouldHaveNoPreviousLink();
        }
    }
}
