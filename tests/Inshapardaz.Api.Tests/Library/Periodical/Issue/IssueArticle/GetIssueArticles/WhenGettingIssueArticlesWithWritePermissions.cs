using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Api.Views;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.Periodical.Issue.IssueArticle.GetIssueArticles
{
    [TestFixture(Role.Admin)]
    [TestFixture(Role.LibraryAdmin)]
    [TestFixture(Role.Writer)]
    public class WhenGettingIssueArticlesWithWritePermissions
        : TestBase
    {
        private IssueDto _issue;
        private HttpResponseMessage _response;
        private ListView<IssueArticleView> _view;

        public WhenGettingIssueArticlesWithWritePermissions(Role role)
            : base(role)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var issues = IssueBuilder.WithLibrary(LibraryId).WithArticles(5).Build(3);
            _issue = RandomData.PickRandom(issues);

            _response = await Client.GetAsync($"/libraries/{LibraryId}/periodicals/{_issue.PeriodicalId}/volumes/{_issue.VolumeNumber}/issues/{_issue.IssueNumber}/articles");
            _view = _response.GetContent<ListView<IssueArticleView>>().Result;
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
            _view.SelfLink()
                .ShouldBeGet()
                .EndingWith($"/libraries/{LibraryId}/periodicals/{_issue.PeriodicalId}/volumes/{_issue.VolumeNumber}/issues/{_issue.IssueNumber}/articles");
        }

        [Test]
        public void ShouldHaveCreateLink()
        {
            _view.CreateLink()
                .ShouldBePost()
                .EndingWith($"/libraries/{LibraryId}/periodicals/{_issue.PeriodicalId}/volumes/{_issue.VolumeNumber}/issues/{_issue.IssueNumber}/articles");
        }

        public void ShouldHaveCorrectNumberOfArticles()
        {
            Assert.That(_view.Data.Count(), Is.EqualTo(5));
        }

        [Test]
        public void ShouldHaveCorrectArticlesData()
        {
            foreach (var expected in IssueBuilder.GetArticles(_issue.Id))
            {
                var actual = _view.Data.FirstOrDefault(x => x.Id == expected.Id);
                var assert = Services.GetService<IssueArticleAssert>().ForView(actual).ForIssueDto(_issue).ForLibrary(LibraryId)
                    .ShouldBeSameAs(expected)
                    .WithWriteableLinks();

                var authors = IssueBuilder.GetAuthorsForIssue(actual.Id);
                foreach (var actualAuthor in actual.Authors)
                {
                    var expectedAuthor = authors.SingleOrDefault(x => x.Id == actualAuthor.Id);
                    actualAuthor.Name.Should().Be(expectedAuthor.Name);
                }
                
                //var contents = ChapterBuilder.Contents.Where(c => c.ChapterId == expected.Id);
                //foreach (var content in contents)
                //{
                //    assert.ShouldHaveContentLink(content);
                //    assert.ShouldHaveUpdateChapterContentLink(content);
                //    assert.ShouldHaveDeleteChapterContentLink(content);
                //}
            }
        }
    }
}
