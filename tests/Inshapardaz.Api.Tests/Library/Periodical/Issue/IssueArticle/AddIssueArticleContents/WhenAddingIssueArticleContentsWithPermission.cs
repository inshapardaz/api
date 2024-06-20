using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.Periodical.Issue.IssueArticle.AddIssueArticleContents
{
    [TestFixture(Role.Admin)]
    [TestFixture(Role.LibraryAdmin)]
    [TestFixture(Role.Writer)]
    public class WhenAddingIssueArticleContentsWithPermission
        : TestBase
    {
        private HttpResponseMessage _response;
        private string _contents;
        private IssueArticleDto _article;
        private IssueArticleContentAssert _assert;

        public WhenAddingIssueArticleContentsWithPermission(Role role)
            : base(role)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var locale = RandomData.Locale;
            var issue = IssueBuilder.WithLibrary(LibraryId).WithArticles(5).WithContentLanguage(locale).Build();
            var articles = IssueBuilder.GetArticles(issue.Id);
            _article = RandomData.PickRandom(articles);
            _contents = RandomData.String;

            _response = await Client.PostString($"/libraries/{LibraryId}/periodicals/{issue.PeriodicalId}/volumes/{issue.VolumeNumber}/issues/{issue.IssueNumber}/articles/{_article.SequenceNumber}/contents?language={RandomData.Locale}", _contents);
            _assert = Services.GetService<IssueArticleContentAssert>().ForResponse(_response).ForIssue(issue).ForLibrary(Library);
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
        public void ShouldSaveTheArticleContent()
        {
            _assert.ShouldHaveSavedArticleContent();
        }

        [Test]
        public void ShouldHaveTextReturened()
        {
            _assert.ShouldHaveText(_contents);
        }

        [Test]
        public void ShouldHaveCorrectTextSaved()
        {
            _assert.ShouldHaveSavedCorrectText(_contents);
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
