using System.Net.Http;
using System.Threading.Tasks;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Models;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.Periodical.Issue.IssueArticle.UpdateIssueArticleAssignment
{
    public class WhenUpdatingIssueArticleWriterAssignmentWithDifferentSequenceNumber
        : TestBase
    {
        private HttpResponseMessage _response;
        private IssueArticleAssert _articleAssert;
        private IssueArticleView _newArticle;

        public WhenUpdatingIssueArticleWriterAssignmentWithDifferentSequenceNumber()
            : base(Role.Writer)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            
            var user1 = AccountBuilder.Build();
            var user2 = AccountBuilder.Build();
            var issue = IssueBuilder.WithLibrary(LibraryId)
                .WithArticles(5)
                .AssignPagesToWriter(user1.Id, 5)
                .WithArticleStatus(EditingStatus.Typing, 5)
                .Build();
            var articles = IssueBuilder.GetArticles(issue.Id);
            var article = RandomData.PickRandom(articles);

            var assignmentView = new AssignmentView { AccountId = user2.Id };

            _response = await Client.PostObject($"/libraries/{LibraryId}/periodicals/{issue.PeriodicalId}/volumes/{issue.VolumeNumber}/issues/{issue.IssueNumber}/articles/{-article.SequenceNumber}/assign", assignmentView);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldHaveBadRequestResult()
        {
            _response.ShouldBeBadRequest();
        }
    }
}
