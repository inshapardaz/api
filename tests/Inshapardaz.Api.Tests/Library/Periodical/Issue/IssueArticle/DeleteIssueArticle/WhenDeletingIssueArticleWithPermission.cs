using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
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
        private IssueArticleAssert _assert;
        private IssueArticleDto _expected;

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
            _assert = Services.GetService<IssueArticleAssert>().ForResponse(_response).ForLibrary(LibraryId);
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
            _assert.ShouldHaveDeletedArticle(_expected.Id);
        }

        [Test]
        public void ShouldHaveDeletedTheChapterContents()
        {
            _assert.ThatContentsAreDeletedForArticle(_expected.Id);
        }
    }
}
