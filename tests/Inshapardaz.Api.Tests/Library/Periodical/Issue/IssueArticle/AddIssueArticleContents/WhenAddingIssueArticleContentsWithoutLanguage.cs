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
    [TestFixture]
    public class WhenAddingIssueArticleContentsWithoutLanguage
        : TestBase
    {
        private HttpResponseMessage _response;
        private IssueArticleDto _article;
        private IssueArticleContentAssert _assert;

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

            _response = await Client.PostString($"/libraries/{LibraryId}/periodicals/{issue.PeriodicalId}/volumes/{issue.VolumeNumber}/issues/{issue.IssueNumber}/articles/{_article.SequenceNumber}/contents", RandomData.String);
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
        public void ShouldHaveUsedLibraryLanguageForContent()
        {
            _assert.ShouldHaveDefaultLibraryLanguage();
        }
    }
}
