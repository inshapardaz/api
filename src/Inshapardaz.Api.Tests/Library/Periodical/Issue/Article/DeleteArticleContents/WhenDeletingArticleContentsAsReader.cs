using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.Helpers;
using Inshapardaz.Domain.Models;
using NUnit.Framework;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.Periodical.Issue.Article.DeleteArticleContents
{
    [TestFixture]
    public class WhenDeletingArticleContentsAsReader
        : TestBase
    {
        private HttpResponseMessage _response;

        public WhenDeletingArticleContentsAsReader()
            : base(Role.Reader)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var issue = IssueBuilder.WithLibrary(LibraryId).WithArticles(1).WithArticleContents(4).Build();
            var article = IssueBuilder.GetArticles(issue.Id).PickRandom();
            var content = IssueBuilder.ArticleContents.Where(x => x.ArticleId == article.Id).PickRandom();

            _response = await Client.DeleteAsync($"/libraries/{LibraryId}/periodicals/{issue.PeriodicalId}/volumes/{issue.VolumeNumber}/issues/{issue.IssueNumber}/articles/{article.SequenceNumber}/contents", content.Language);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldHaveForbiddenResult()
        {
            _response.ShouldBeForbidden();
        }
    }
}
