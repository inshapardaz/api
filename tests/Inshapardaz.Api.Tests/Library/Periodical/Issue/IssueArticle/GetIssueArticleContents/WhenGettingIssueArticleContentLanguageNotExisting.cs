using Inshapardaz.Api.Tests.Helpers;
using Inshapardaz.Api.Tests.Asserts;
using NUnit.Framework;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Inshapardaz.Domain.Models;

namespace Inshapardaz.Api.Tests.Library.Periodical.Issue.IssueArticle.GetIssueArticleContents
{
    [TestFixture]
    public class WhenGettingIssueArticleContentLanguageNotExisting
        : TestBase
    {
        private HttpResponseMessage _response;

        public WhenGettingIssueArticleContentLanguageNotExisting()
            : base(Role.Reader)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var issue = IssueBuilder.WithLibrary(LibraryId).WithArticles(1).WithArticleContents(4).Build();
            var article = IssueBuilder.GetArticles(issue.Id).PickRandom();
            var content = IssueBuilder.ArticleContents.Where(x => x.ArticleId == article.Id).PickRandom();

            _response = await Client.GetAsync($"/libraries/{LibraryId}/periodicals/{issue.PeriodicalId}/volumes/{issue.VolumeNumber}/issues/{issue.IssueNumber}/articles/{article.SequenceNumber}/contents", "test");
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
