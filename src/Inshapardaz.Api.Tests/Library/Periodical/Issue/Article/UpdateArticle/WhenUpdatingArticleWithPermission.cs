using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.Helpers;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Models;
using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.Periodical.Issue.Article.UpdateArticle
{
    [TestFixture(Role.Admin)]
    [TestFixture(Role.LibraryAdmin)]
    [TestFixture(Role.Writer)]
    public class WhenUpdatingArticleWithPermission
        : TestBase
    {
        private HttpResponseMessage _response;
        private IssueArticleAssert _articleAssert;
        private IssueArticleView _newArticle;

        public WhenUpdatingArticleWithPermission(Role role)
            : base(role)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var issue = IssueBuilder.WithLibrary(LibraryId).WithArticles(5).Build();

            var oldArticle = IssueBuilder.GetArticles(issue.Id).PickRandom();

            _newArticle = new IssueArticleView { 
                Title = RandomData.Name,
                SeriesName = oldArticle.SeriesName,
                SeriesIndex = oldArticle.SeriesIndex,
                SequenceNumber = oldArticle.SequenceNumber
            };

            _response = await Client.PutObject($"/libraries/{LibraryId}/periodicals/{issue.PeriodicalId}/volumes/{issue.VolumeNumber}/issues/{issue.IssueNumber}/articles/{_newArticle.SequenceNumber}", _newArticle);

            _articleAssert = IssueArticleAssert.FromResponse(_response, LibraryId, issue);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldHaveOKResult()
        {
            _response.ShouldBeOk();
        }

        [Test]
        public void ShouldHaveReturnedUpdatedArticle()
        {
            IssueArticleView expected = new IssueArticleView
            {
                Title = _newArticle.Title,
                SequenceNumber = _newArticle.SequenceNumber,
                SeriesName = _newArticle.SeriesName, 
                SeriesIndex = _newArticle.SeriesIndex,
                Status = "Available"
            };
            _articleAssert.ShouldMatch(expected);
        }

        [Test]
        public void ShouldHaveUpdatedArticle()
        {
            _articleAssert.ShouldHaveSavedArticle(DatabaseConnection);
        }
    }
}
