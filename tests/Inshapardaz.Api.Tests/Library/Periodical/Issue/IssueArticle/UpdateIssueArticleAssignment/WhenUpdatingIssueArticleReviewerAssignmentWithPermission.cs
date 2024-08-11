using System;
using System.Net.Http;
using System.Threading.Tasks;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.Periodical.Issue.IssueArticle.UpdateIssueArticleAssignment
{
    [TestFixture(Role.Admin)]
    [TestFixture(Role.LibraryAdmin)]
    [TestFixture(Role.Writer)]
    public class WhenUpdatingIssueArticleReviewerAssignmentWithPermission
        : TestBase
    {
        private HttpResponseMessage _response;
        private IssueArticleAssert _articleAssert;
        private IssueArticleDto _article;
        private AssignmentView _assignmentView;

        public WhenUpdatingIssueArticleReviewerAssignmentWithPermission(Role role)
            : base(role)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var user1 = AccountBuilder.InLibrary(LibraryId).Build();
            var user2 = AccountBuilder.InLibrary(LibraryId).Build();
            var issue = IssueBuilder.WithLibrary(LibraryId)
                .WithArticles(5)
                .AssignPagesToWriter(user1.Id, 5)
                .WithArticleStatus(EditingStatus.InReview, 5)
                .Build();
            var articles = IssueBuilder.GetArticles(issue.Id);
            _article = RandomData.PickRandom(articles);

            _assignmentView = new AssignmentView { AccountId = user2.Id };

            _response = await Client.PostObject($"/libraries/{LibraryId}/periodicals/{issue.PeriodicalId}/volumes/{issue.VolumeNumber}/issues/{issue.IssueNumber}/articles/{_article.SequenceNumber}/assign", _assignmentView);

            _articleAssert = Services.GetService<IssueArticleAssert>().ForResponse(_response)
                    .ForLibrary(LibraryId)
                    .ForIssueDto(issue);
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
                Title = _article.Title,
                SequenceNumber = _article.SequenceNumber,
                SeriesName = _article.SeriesName, 
                SeriesIndex = _article.SeriesIndex,
                Status = "InReview",
                WriterAccountId = _article.WriterAccountId,
                WriterAssignTimeStamp = _article.WriterAssignTimeStamp,
                ReviewerAccountId = _assignmentView.AccountId,
                ReviewerAssignTimeStamp = DateTime.UtcNow
            };
            _articleAssert.ShouldMatch(expected);
        }

        [Test]
        public void ShouldHaveUpdatedArticle()
        {
            _articleAssert.ShouldHaveSavedArticle();
        }
    }
}
