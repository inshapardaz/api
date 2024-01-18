using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.Helpers;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Models;
using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.Periodical.Issue.IssueArticle.UpdateIssueArticle
{
    public class WhenUpdatingIssueArticleWithDifferentSequenceNumber
        : TestBase
    {
        private HttpResponseMessage _response;
        private IssueArticleAssert _articleAssert;
        private IssueArticleView _newArticle;

        public WhenUpdatingIssueArticleWithDifferentSequenceNumber()
            : base(Role.Writer)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var issue = IssueBuilder.WithLibrary(LibraryId).WithArticles(5).Build();

            _newArticle = new IssueArticleView { Title = RandomData.Name, SequenceNumber = 1 };

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
