using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.Dto;
using Inshapardaz.Domain.Models;
using NUnit.Framework;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.Periodical.Issue.IssueArticle.GetIssueArticleById
{
    [TestFixture]
    public class WhenGettingFirstArticleOfIssue
        : TestBase
    {
        private HttpResponseMessage _response;
        private IssueArticleDto _expected;
        private IssueArticleAssert _assert;

        public WhenGettingFirstArticleOfIssue()
            : base(Role.Reader)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var issue = IssueBuilder.WithLibrary(LibraryId).WithArticles(5).Build();
            _expected = IssueBuilder.GetArticles(issue.Id).First();

            _response = await Client.GetAsync($"/libraries/{LibraryId}/periodicals/{issue.PeriodicalId}/volumes/{issue.VolumeNumber}/issues/{issue.IssueNumber}/articles/{_expected.SequenceNumber}");

            _assert = IssueArticleAssert.FromResponse(_response, LibraryId, issue);
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
                  .ShouldHaveIssueLink();
        }

        [Test]
        public void ShouldNotHavePreviousLink()
        {
            _assert.ShouldHaveNotPreviousLink();
        }

        [Test]
        public void ShouldHaveNextLink()
        {
            _assert.ShouldHaveNextLink(_expected.SequenceNumber + 1);
        }
    }
}
