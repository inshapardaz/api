using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.Periodical.Issue.IssueArticle.GetIssueArticleContents
{
    [TestFixture(Role.Admin)]
    [TestFixture(Role.LibraryAdmin)]
    [TestFixture(Role.Writer)]
    public class WhenGettingIssueArticleContentWithPermission
        : TestBase
    {
        private HttpResponseMessage _response;
        private IssueArticleContentAssert _assert;
        private IssueDto _issue;
        private IssueArticleDto _article;
        private IssueArticleContentDto _content;

        public WhenGettingIssueArticleContentWithPermission(Role role)
            : base(role)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            _issue = IssueBuilder.WithLibrary(LibraryId).WithArticles(1).WithArticleContents(4).Build();
            _article = IssueBuilder.GetArticles(_issue.Id).PickRandom();
            _content = IssueBuilder.ArticleContents.Where(x => x.ArticleId == _article.Id).PickRandom();

            _response = await Client.GetAsync($"/libraries/{LibraryId}/periodicals/{_issue.PeriodicalId}/volumes/{_issue.VolumeNumber}/issues/{_issue.IssueNumber}/articles/{_article.SequenceNumber}/contents?language={_content.Language}");
            _assert = Services.GetService<IssueArticleContentAssert>().ForResponse(_response).ForLibrary(Library);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldReturnOk()
        {
            _response.ShouldBeOk();
        }

        [Test]
        public void ShouldHaveSelfLink()
        {
            _assert.ShouldHaveSelfLink();
        }

        [Test]
        public void ShouldHavePeriodicalLink()
        {
            _assert.ShouldHavePeriodicalLink();
        }

        [Test]
        public void ShouldHaveIssueLink()
        {
            _assert.ShouldHaveIssueLink();
        }


        [Test]
        public void ShouldHaveArticleLink()
        {
            _assert.ShouldHaveArticleLink();
        }

        [Test]
        public void ShouldHaveEditLinks()
        {
            _assert.ShouldHaveUpdateLink()
                .ShouldHaveDeleteLink();
        }


        [Test]
        public void ShouldHaveTextReturened()
        {
            var file = FileTestRepository.GetFileById(_content.FileId.Value);
            var fileText = FileStore.GetTextFile(file.FilePath, CancellationToken.None).Result;
            _assert.ShouldHaveText(fileText);
        }

        [Test]
        public void ShouldReturnCorrectArticleData()
        {
            _assert.ShouldMatch(_content, _issue, _article);
        }
    }
}
