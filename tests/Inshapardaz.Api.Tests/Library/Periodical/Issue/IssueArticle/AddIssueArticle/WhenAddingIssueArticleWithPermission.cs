using System.Linq;
using Bogus;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.Periodical.Issue.IssueArticle.AddIssueArticle
{
    [TestFixture(Role.Admin)]
    [TestFixture(Role.LibraryAdmin)]
    [TestFixture(Role.Writer)]
    public class WhenAddingIssueArticleWithPermission
        : TestBase
    {
        private IssueArticleView _article;
        private HttpResponseMessage _response;
        private IssueArticleAssert _assert;

        public WhenAddingIssueArticleWithPermission(Role role)
            : base(role)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var authors = AuthorBuilder.WithLibrary(LibraryId).Build(3);
            var issue = IssueBuilder.WithLibrary(LibraryId).WithArticles(3)
                .Build();

            _article = new IssueArticleView
            {
                Title = new Faker().Random.Words(2),
                Authors = authors.Select(x => new AuthorView { Id = x.Id })
            };

            _response = await Client.PostObject($"/libraries/{LibraryId}/periodicals/{issue.PeriodicalId}/volumes/{issue.VolumeNumber}/issues/{issue.IssueNumber}/articles", _article);

            _assert = Services.GetService<IssueArticleAssert>().ForResponse(_response).ForDto(issue).ForLibrary(LibraryId);
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
        public void ShouldSaveTheIssue()
        {
            _assert.ShouldHaveSavedArticle();
        }

        [Test]
        public void ShouldHaveCorrectObjectReturned()
        {
            _assert.ShouldMatch(new IssueArticleView { 
                Title = _article.Title,
                SequenceNumber = 4,
                Status = "Available"
            });
        }
        
        [Test]
        public void ShouldHaveCorrectObjectSaved()
        {
            _assert.ShouldHaveSavedArticle();
        }

        [Test]
        public void ShouldHaveLinks()
        {
            _assert.ShouldHaveSelfLink()
                   .ShouldHavePeriodicalLink()
                   .ShouldHaveIssueLink()
                   .ShouldHaveUpdateLink()
                   .ShouldHaveDeleteLink()
                   .ShouldNotHaveContentsLink()
                   .ShouldHaveAddIssueContentLink();
        }
    }
}
