using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.Dto;
using Inshapardaz.Api.Tests.Helpers;
using Inshapardaz.Domain.Models;
using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.Periodical.Issue.IssueArticle.AddIssueArticleContents
{
    [TestFixture]
    public class WhenAddingIssueArticleContentsWithoutLanguage
        : TestBase
    {
        private HttpResponseMessage _response;
        private IssueArticleDto _article;
        private ArticleContentAssert _assert;

        public WhenAddingIssueArticleContentsWithoutLanguage()
            : base(Role.Writer)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var issue = IssueBuilder.WithLibrary(LibraryId).WithArticles(5).Build();
            var articles = IssueBuilder.GetArticles(issue.Id);
            _article = RandomData.PickRandom(articles);

            _response = await Client.PostString($"/libraries/{LibraryId}/periodicals/{issue.PeriodicalId}/volumes/{issue.VolumeNumber}/issues/{issue.IssueNumber}/articles/{_article.SequenceNumber}/contents", RandomData.String, null);
            _assert = new ArticleContentAssert(_response, Library, issue);
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
        public void ShouldHaveUsedLibraryLanguageForContent()
        {
            _assert.ShouldHaveDefaultLibraryLanguage();
        }
    }
}
