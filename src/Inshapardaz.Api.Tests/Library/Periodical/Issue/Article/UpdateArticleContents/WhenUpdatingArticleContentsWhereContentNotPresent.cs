using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.Dto;
using Inshapardaz.Api.Tests.Helpers;
using Inshapardaz.Domain.Models;
using NUnit.Framework;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.Periodical.Issue.Article.UpdateArticleContents
{
    [TestFixture]
    public class WhenUpdatingArticleContentsWhereContentNotPresent
        : TestBase
    {
        private HttpResponseMessage _response;
        private ArticleContentAssert _assert;
        private IssueDto _issue;
        private ArticleDto _article;
        private string _newContents;

        public WhenUpdatingArticleContentsWhereContentNotPresent()
            : base(Role.Writer)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            _newContents = RandomData.String;

            _issue = IssueBuilder.WithLibrary(LibraryId).WithArticles(1).Build();
            _article = IssueBuilder.GetArticles(_issue.Id).PickRandom();

            _response = await Client.PutString($"/libraries/{LibraryId}/periodicals/{_issue.PeriodicalId}/volumes/{_issue.VolumeNumber}/issues/{_issue.IssueNumber}/articles/{_article.SequenceNumber}/contents", _newContents, RandomData.Locale);

            _assert = new ArticleContentAssert(_response, LibraryId, _issue);
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
        public void ShouldHaveLocationHeader()
        {
            _assert.ShouldHaveCorrectLocationHeader();
        }

        [Test]
        public void ShouldHaveSavedArticleContent()
        {
            _assert.ShouldHaveSavedArticleContent(DatabaseConnection);
        }

        [Test]
        public void ShouldHaveTextReturened()
        {
            _assert.ShouldHaveText(_newContents);
        }

        [Test]
        public void ShouldHaveCorrectContentSaved()
        {
            _assert.ShouldHaveSavedCorrectText(_newContents, DatabaseConnection);
        }

        [Test]
        public void ShouldHaveLinks()
        {
            _assert.ShouldHaveSelfLink()
                   .ShouldHavePeriodicalLink()
                   .ShouldHaveIssueLink()
                   .ShouldHaveArticleLink()
                   .ShouldHaveUpdateLink()
                   .ShouldHaveDeleteLink();
        }
    }
}
