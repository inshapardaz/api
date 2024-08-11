using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.Periodical.Issue.IssueArticle.GetIssueArticleById
{
    [TestFixture]
    public class WhenGettingIssueArticleByIdAsReader
        : TestBase
    {
        private HttpResponseMessage _response;
        private IssueArticleDto _expected;
        private IssueArticleAssert _assert;

        public WhenGettingIssueArticleByIdAsReader()
            : base(Role.Reader)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var issue = IssueBuilder.WithLibrary(LibraryId).WithArticles(5).Build();
            var articles = IssueBuilder.GetArticles(issue.Id);
            _expected = RandomData.PickRandom(articles);

            _response = await Client.GetAsync($"/libraries/{LibraryId}/periodicals/{issue.PeriodicalId}/volumes/{issue.VolumeNumber}/issues/{issue.IssueNumber}/articles/{_expected.SequenceNumber}");
            _assert = Services.GetService<IssueArticleAssert>().ForResponse(_response).ForLibrary(LibraryId).ForIssueDto(issue);
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
        public void ShouldHaveCorrectObjectReturned()
        {
            var authors = IssueBuilder.GetAuthorsForIssue(_expected.Id);
            _assert.ShouldMatch(_expected, authors);
        }

        [Test]
        public void ShouldHaveLinks()
        {
            _assert.ShouldHaveSelfLink()
                   .ShouldHavePeriodicalLink()
                   .ShouldHaveIssueLink();
        }

        [Test]
        public void ShouldNotHaveEditLinks()
        {
            _assert.ShouldNotHaveUpdateLink()
                   .ShouldNotHaveDeleteLink()
                   .ShouldNotHaveAddArticleContentLink()
                   .ShouldNotHaveAssignmentLink();
        }

        [Test]
        public void ShouldNotHaveContentsLink()
        {
            _assert.ShouldHaveNoCorrectContents();
        }
    }
}
