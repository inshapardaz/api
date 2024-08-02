using System.Net.Http;
using System.Threading.Tasks;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Models;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.Periodical.Issue.IssueArticle.UpdateIssueArticleAssignment
{
    [TestFixture]
    public class WhenUpdatingIssueArticleReviewerAssignmentAndAccountDoesNotExist
        : TestBase
    {
        private HttpResponseMessage _response;

        public WhenUpdatingIssueArticleReviewerAssignmentAndAccountDoesNotExist()
            : base(Role.Writer)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var issue = IssueBuilder.WithLibrary(LibraryId)
                .WithArticles(5)
                .WithArticleStatus(EditingStatus.InReview, 5)
                .Build();
            var articles = IssueBuilder.GetArticles(issue.Id);
            var article = RandomData.PickRandom(articles);
            var assignmentView = new AssignmentView { AccountId = -RandomData.Number };
            
            _response = await Client.PostObject($"/libraries/{LibraryId}/periodicals/{issue.PeriodicalId}/volumes/{issue.VolumeNumber}/issues/{issue.IssueNumber}/articles/{article.SequenceNumber}/assign", assignmentView);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldHaveBadRequest()
        {
            _response.ShouldBeBadRequest();
        }
    }
}
