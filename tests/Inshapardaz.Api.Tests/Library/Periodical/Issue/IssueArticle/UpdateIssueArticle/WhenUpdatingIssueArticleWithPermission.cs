using System.Linq;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Inshapardaz.Api.Tests.Framework.DataHelpers;
using Inshapardaz.Api.Tests.Framework.Dto;

namespace Inshapardaz.Api.Tests.Library.Periodical.Issue.IssueArticle.UpdateIssueArticle
{
    [TestFixture(Role.Admin)]
    [TestFixture(Role.LibraryAdmin)]
    [TestFixture(Role.Writer)]
    public class WhenUpdatingIssueArticleWithPermission
        : TestBase
    {
        private HttpResponseMessage _response;
        private IssueArticleAssert _articleAssert;
        private IssueArticleView _newArticle;
        private IssueDto _issue;
        private IssueArticleDto _oldArticle;

        public WhenUpdatingIssueArticleWithPermission(Role role)
            : base(role)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            _issue = IssueBuilder.WithLibrary(LibraryId).WithArticles(5).Build();

            _oldArticle = IssueBuilder.GetArticles(_issue.Id).PickRandom();

            _newArticle = new IssueArticleView { 
                Title = RandomData.Name,
                SeriesName = _oldArticle.SeriesName,
                SeriesIndex = _oldArticle.SeriesIndex,
                SequenceNumber = _oldArticle.SequenceNumber
            };

            _response = await Client.PutObject($"/libraries/{LibraryId}/periodicals/{_issue.PeriodicalId}/volumes/{_issue.VolumeNumber}/issues/{_issue.IssueNumber}/articles/{_newArticle.SequenceNumber}", _newArticle);

            _articleAssert = Services.GetService<IssueArticleAssert>().ForResponse(_response)
                    .ForLibrary(LibraryId)
                    .ForIssueDto(_issue);
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
            _articleAssert.ShouldHaveSavedArticle();
        }

        [Test]
        public void ShouldNotHaveUpdatedOtherArticles()
        {
            var articles = IssueBuilder.GetArticles(_issue.Id);
            foreach (var article in articles.Where(x => x.Id != _oldArticle.Id))
            {
                var dbArticle = Services.GetService<IIssueArticleTestRepository>().GetIssueArticleById(article.Id);
                article.Id.Should().Be(dbArticle.Id);
                article.Title.Should().Be(dbArticle.Title);
                article.Status.Should().Be(dbArticle.Status);
                article.IssueId.Should().Be(dbArticle.IssueId);
                article.SequenceNumber.Should().Be(dbArticle.SequenceNumber);
                article.SeriesName.Should().Be(dbArticle.SeriesName);
                article.SeriesIndex.Should().Be(dbArticle.SeriesIndex);
                article.WriterAccountId.Should().Be(dbArticle.WriterAccountId);
                article.WriterAssignTimeStamp.Should().Be(dbArticle.WriterAssignTimeStamp);
                article.ReviewerAccountId.Should().Be(dbArticle.ReviewerAccountId);
                article.ReviewerAssignTimeStamp.Should().Be(dbArticle.ReviewerAssignTimeStamp);
            }
        }
    }
}
