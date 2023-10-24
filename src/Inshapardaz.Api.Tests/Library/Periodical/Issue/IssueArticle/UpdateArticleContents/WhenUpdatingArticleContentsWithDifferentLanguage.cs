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
    public class WhenUpdatingArticleContentsWithDifferentLanguage
        : TestBase
    {
        private HttpResponseMessage _response;
        private ArticleContentAssert _assert;
        private IssueDto _issue;
        private IssueArticleDto _article;
        private string _newArticleContentLanguage;
        private string _newContents;

        public WhenUpdatingArticleContentsWithDifferentLanguage()
            : base(Role.Writer)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            _newContents = RandomData.String;

            _issue = IssueBuilder.WithLibrary(LibraryId).WithArticles(1).WithArticleContents(2).WithContentLanguage("en").Build();
            _article = IssueBuilder.GetArticles(_issue.Id).PickRandom();
            _newArticleContentLanguage = "ur";

            _response = await Client.PutString($"/libraries/{LibraryId}/periodicals/{_issue.PeriodicalId}/volumes/{_issue.VolumeNumber}/issues/{_issue.IssueNumber}/articles/{_article.SequenceNumber}/contents", _newContents, _newArticleContentLanguage);

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
        public void ShouldHaveLinks()
        {
            _assert.ShouldHaveSelfLink()
                   .ShouldHavePeriodicalLink()
                   .ShouldHaveIssueLink()
                   .ShouldHaveArticleLink()
                   .ShouldHaveUpdateLink()
                   .ShouldHaveDeleteLink();
        }

        [Test]
        public void ShouldHaveTextReturened()
        {
            _assert.ShouldHaveText(_newContents);
        }

        [Test]
        public void ShouldHaveUpdatedContents()
        {
            _assert.ShouldHaveMatechingTextForLanguage(_newContents, _newArticleContentLanguage, DatabaseConnection);
        }
    }
}
