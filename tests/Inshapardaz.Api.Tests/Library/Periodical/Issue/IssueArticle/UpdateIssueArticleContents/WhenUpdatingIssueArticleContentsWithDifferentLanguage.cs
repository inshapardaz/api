using System.Net.Http;
using System.Threading.Tasks;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.Periodical.Issue.IssueArticle.UpdateIssueArticleContents
{
    [TestFixture]
    public class WhenUpdatingIssueArticleContentsWithDifferentLanguage
        : TestBase
    {
        private HttpResponseMessage _response;
        private IssueArticleContentAssert _assert;
        private IssueDto _issue;
        private IssueArticleDto _article;
        private string _newArticleContentLanguage;
        private string _newContents;

        public WhenUpdatingIssueArticleContentsWithDifferentLanguage()
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

            _assert = Services.GetService<IssueArticleContentAssert>().ForResponse(_response).ForIssue(_issue).ForLibrary(Library);
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
            _assert.ShouldHaveMatechingTextForLanguage(_newContents, _newArticleContentLanguage);
        }
    }
}
