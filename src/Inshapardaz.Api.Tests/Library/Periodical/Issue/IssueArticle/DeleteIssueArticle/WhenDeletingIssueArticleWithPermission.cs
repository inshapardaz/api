using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.Dto;
using Inshapardaz.Api.Tests.Helpers;
using Inshapardaz.Domain.Models;
using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.Periodical.Issue.IssueArticle.DeleteIssueArticle
{
    [TestFixture(Role.Admin)]
    [TestFixture(Role.LibraryAdmin)]
    [TestFixture(Role.Writer)]
    public class WhenDeletingIssueArticleWithPermission
        : TestBase
    {
        private HttpResponseMessage _response;
        private ArticleDto _expected;

        public WhenDeletingIssueArticleWithPermission(Role role)
            : base(role)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var issue = IssueBuilder.WithLibrary(LibraryId).WithArticles(5).Build();
            var articles = IssueBuilder.GetArticles(issue.Id);
            _expected = RandomData.PickRandom(articles);
            _response = await Client.DeleteAsync($"/libraries/{LibraryId}/periodicals/{issue.PeriodicalId}/volumes/{issue.VolumeNumber}/issues/{issue.IssueNumber}/articles/{_expected.SequenceNumber}");
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldReturnNoContent()
        {
            _response.ShouldBeNoContent();
        }

        [Test]
        public void ShouldHaveDeletedArticle()
        {
            IssueArticleAssert.ShouldHaveDeletedArticle(_expected.Id, DatabaseConnection);
        }

        [Test]
        public void ShouldHaveDeletedTheChapterContents()
        {
            IssueArticleAssert.ThatContentsAreDeletedForArticle(_expected.Id, DatabaseConnection);
        }
    }
}
