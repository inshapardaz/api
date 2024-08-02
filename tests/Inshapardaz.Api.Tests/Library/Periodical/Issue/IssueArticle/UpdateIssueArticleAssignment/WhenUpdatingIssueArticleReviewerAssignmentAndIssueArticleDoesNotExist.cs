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
    public class WhenUpdatingIssueArticleReviewerAssignmentAndIssueArticleDoesNotExist
        : TestBase
    {
        private HttpResponseMessage _response;

        public WhenUpdatingIssueArticleReviewerAssignmentAndIssueArticleDoesNotExist()
            : base(Role.Writer)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var user1 = AccountBuilder.Build();
            var issue = IssueBuilder.WithLibrary(LibraryId)
                .WithArticles(5)
                .WithArticleStatus(EditingStatus.InReview, 5)
                .Build();

            var assignmentView = new AssignmentView { AccountId = user1.Id };
            
            _response = await Client.PostObject($"/libraries/{LibraryId}/periodicals/{issue.PeriodicalId}/volumes/{issue.VolumeNumber}/issues/{issue.IssueNumber}/articles/{-RandomData.Number}/assign", assignmentView);
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
