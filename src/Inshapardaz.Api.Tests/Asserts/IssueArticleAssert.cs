using FluentAssertions;
using Inshapardaz.Api.Tests.DataHelpers;
using Inshapardaz.Api.Tests.Dto;
using Inshapardaz.Api.Tests.Helpers;
using Inshapardaz.Api.Views.Library;
using System;
using System.Data;
using System.Linq;
using System.Net.Http;

namespace Inshapardaz.Api.Tests.Asserts
{
    internal class IssueArticleAssert
    {
        private HttpResponseMessage _response;
        private readonly int _libraryId;
        private readonly IssueDto _issue;
        private IssueArticleView _article;

        public IssueArticleAssert(IssueArticleView view, int libraryId, IssueDto issue)
        {
            _libraryId = libraryId;
            _issue = issue;
            _article = view;
        }

        public IssueArticleAssert(HttpResponseMessage response, int libraryId, IssueDto issue)
        {
            _response = response;
            _libraryId = libraryId;
            _issue = issue;
            _article = response.GetContent<IssueArticleView>().Result;
        }

        internal static IssueArticleAssert FromResponse(HttpResponseMessage response, int libraryId, IssueDto issue)
        {
            return new IssueArticleAssert(response, libraryId, issue);
        }

        internal static IssueArticleAssert FromObject(IssueArticleView view, int libraryId, IssueDto issue)
        {
            return new IssueArticleAssert(view, libraryId, issue);
        }

        internal IssueArticleAssert ShouldHaveCorrectLocationHeader()
        {
            var location = _response.Headers.Location.AbsoluteUri;
            location.Should().NotBeNull();
            location.Should().EndWith($"libraries/{_libraryId}/periodicals/{_issue.PeriodicalId}/volumes/{_issue.VolumeNumber}/issues/{_issue.IssueNumber}/articles/{_article.SequenceNumber}");
            return this;
        }

        internal IssueArticleAssert ShouldBeAssignedToUserForWriting(AccountDto account)
        {
            _article.WriterAccountId.Should().Be(account.Id);
            _article.WriterAccountName.Should().Be(account.Name);
            _article.WriterAssignTimeStamp.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(2));
            return this;
        }

        internal IssueArticleAssert ShouldNotBeAssignedForWriting()
        {
            _article.WriterAccountId.Should().BeNull();
            _article.WriterAccountName.Should().BeNull();
            _article.WriterAssignTimeStamp.Should().BeNull();
            return this;
        }

        internal IssueArticleAssert ShouldBeSavedAssignmentForWriting(IDbConnection dbConnection, AccountDto account)
        {
            var dbArticle = dbConnection.GetArticleById(_article.Id);
            dbArticle.WriterAccountId.Should().Be(account.Id);
            dbArticle.WriterAssignTimestamp.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(2));
            return this;
        }

        internal IssueArticleAssert ShouldBeSavedNoAssignmentForWriting(IDbConnection dbConnection)
        {
            var dbArticle = dbConnection.GetArticleById(_article.Id);
            dbArticle.WriterAccountId.Should().BeNull();
            dbArticle.WriterAssignTimestamp.Should().BeNull();
            return this;
        }

        internal IssueArticleAssert ShouldBeAssignedToUserForReviewing(AccountDto account)
        {
            _article.ReviewerAccountId.Should().Be(account.Id);
            _article.ReviewerAccountName.Should().Be(account.Name);
            _article.ReviewerAssignTimeStamp.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(2));
            return this;
        }

        internal IssueArticleAssert ShouldNotBeAssignedForReviewing()
        {
            _article.ReviewerAccountId.Should().BeNull();
            _article.ReviewerAccountName.Should().BeNull();
            _article.ReviewerAssignTimeStamp.Should().BeNull();
            return this;
        }

        internal IssueArticleAssert ShouldBeSavedAssignmentForReviewing(IDbConnection dbConnection, AccountDto account)
        {
            var dbArticle = dbConnection.GetArticleById(_article.Id);
            dbArticle.ReviewerAccountId.Should().Be(account.Id);
            dbArticle.ReviewerAssignTimestamp.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(2));
            return this;
        }

        internal IssueArticleAssert ShouldBeSavedNoAssignmentForReviewing(IDbConnection dbConnection)
        {
            var dbArticle = dbConnection.GetArticleById(_article.Id);
            dbArticle.ReviewerAccountId.Should().BeNull();
            dbArticle.ReviewerAssignTimestamp.Should().BeNull();
            return this;
        }

        internal IssueArticleAssert ShouldHaveSavedArticle(IDbConnection dbConnection)
        {
            var dbArticle = dbConnection.GetArticleById(_article.Id);
            dbArticle.Should().NotBeNull();
            _article.Title.Should().Be(dbArticle.Title);
            return this;
        }

        internal static void ShouldHaveDeletedArticle(int articleId, IDbConnection databaseConnection)
        {
            var article = databaseConnection.GetArticleById(articleId);
            article.Should().BeNull();
        }

        internal static void ThatContentsAreDeletedForArticle(int articleId, IDbConnection databaseConnection)
        {
            var contents = databaseConnection.GetContentByArticle(articleId);
            contents.Should().BeNullOrEmpty();
        }

        internal IssueArticleAssert ShouldHaveSelfLink()
        {
            _article.SelfLink()
                  .ShouldBeGet()
                  .EndingWith($"libraries/{_libraryId}/periodicals/{_issue.PeriodicalId}/volumes/{_issue.VolumeNumber}/issues/{_issue.IssueNumber}/articles/{_article.SequenceNumber}");

            return this;
        }

        internal IssueArticleAssert WithReadOnlyLinks()
        {
            ShouldHaveSelfLink()
            .ShouldHavePeriodicalLink()
            .ShouldHaveIssueLink()
            .ShouldNotHaveAddArticleContentLink()
            .ShouldNotHaveUpdateLink()
            .ShouldNotHaveDeleteLink()
            .ShouldNotHaveAssignmentLink();
            return this;
        }

        internal IssueArticleAssert WithWriteableLinks()
        {
            ShouldHaveAddIssueContentLink()
            .ShouldHaveUpdateLink()
            .ShouldHaveDeleteLink()
            .ShouldHaveAssignmentLink();
            return this;
        }

        internal void ShouldHaveContentLink(ChapterContentDto content)
        {
            var actual = _article.Contents.Single(x => x.Id == content.Id);
            actual.SelfLink()
                  .ShouldBeGet()
                  .ShouldHaveAcceptLanguage(content.Language);
        }

        internal void ShouldHaveNoCorrectContents()
        {
            _article.Link("content").Should().BeNull();
        }
        internal IssueArticleAssert ShouldHaveIssueLink()
        {
            _article.Link("issue")
                  .ShouldBeGet()
                  .EndingWith($"libraries/{_libraryId}/periodicals/{_issue.PeriodicalId}/volumes/{_issue.VolumeNumber}/issues/{_issue.IssueNumber}");

            return this;
        }

        internal IssueArticleAssert ShouldHavePeriodicalLink()
        {
            _article.Link("periodical")
                  .ShouldBeGet()
                  .EndingWith($"libraries/{_libraryId}/periodicals/{_issue.PeriodicalId}");

            return this;
        }

        internal IssueArticleAssert ShouldHaveAssignmentLink()
        {
            _article.Link("assign")
                  .ShouldBePost()
                  .EndingWith($"libraries/{_libraryId}/periodicals/{_issue.PeriodicalId}/volumes/{_issue.VolumeNumber}/issues/{_issue.IssueNumber}/articles/{_article.SequenceNumber}/assign");
            return this;
        }

        internal IssueArticleAssert ShouldNotHaveAssignmentLink()
        {
            _article.Link("assign")
                  .Should().BeNull();
            return this;
        }

        internal void ShouldHaveCorrectContents(IDbConnection db)
        {
            var contents = db.GetContentByChapter(_article.Id);

            contents.Should().HaveSameCount(_article.Contents);

            foreach (var content in contents)
            {
                ShouldHaveContentLink(content);
            }
        }

        internal IssueArticleAssert ShouldHaveUpdateLink()
        {
            _article.UpdateLink()
                 .ShouldBePut()
                 .EndingWith($"libraries/{_libraryId}/periodicals/{_issue.PeriodicalId}/volumes/{_issue.VolumeNumber}/issues/{_issue.IssueNumber}/articles/{_article.SequenceNumber}");

            return this;
        }

        internal IssueArticleAssert ShouldNotHaveUpdateLink()
        {
            _article.UpdateLink().Should().BeNull();
            return this;
        }

        internal IssueArticleAssert ShouldHaveDeleteLink()
        {
            _article.DeleteLink()
                 .ShouldBeDelete()
                 .EndingWith($"libraries/{_libraryId}/periodicals/{_issue.PeriodicalId}/volumes/{_issue.VolumeNumber}/issues/{_issue.IssueNumber}/articles/{_article.SequenceNumber}");

            return this;
        }

        internal IssueArticleAssert ShouldNotHaveDeleteLink()
        {
            _article.DeleteLink().Should().BeNull();
            return this;
        }

        internal IssueArticleAssert ShouldHaveAddIssueContentLink()
        {
            _article.Link("add-content")
                 .ShouldBePost()
                 .EndingWith($"libraries/{_libraryId}/periodicals/{_issue.PeriodicalId}/volumes/{_issue.VolumeNumber}/issues/{_issue.IssueNumber}/articles/{_article.SequenceNumber}/contents");

            return this;
        }

        internal IssueArticleAssert ShouldNotHaveAddArticleContentLink()
        {
            _article.Link("add-content").Should().BeNull();
            return this;
        }

        internal IssueArticleAssert ShouldHaveUpdateArticleContentLink(IssueContentDto content)
        {
            var actual = _article.Contents.Single(x => x.Id == content.Id);
            actual.UpdateLink()
                  .ShouldBePut()
                  .EndingWith($"libraries/{_libraryId}/periodicals/{_issue.PeriodicalId}/volumes/{_issue.VolumeNumber}/issues/{_issue.IssueNumber}/articles/{_article.SequenceNumber}/contents")
                  .ShouldHaveAcceptLanguage(content.Language);

            return this;
        }

        internal IssueArticleAssert ShouldHaveDeleteIssueContentLink(IssueContentDto content)
        {
            var actual = _article.Contents.Single(x => x.Id == content.Id);
            actual.DeleteLink()
                  .ShouldBeDelete()
                  .EndingWith($"libraries/{_libraryId}/periodicals/{_issue.PeriodicalId}/volumes/{_issue.VolumeNumber}/issues/{_issue.IssueNumber}/articles/{_article.SequenceNumber}/contents")
                  .ShouldHaveAcceptLanguage(actual.Language);

            return this;
        }

        internal IssueArticleAssert ShouldNotHaveContentsLink()
        {
            _article.Link("content").Should().BeNull();
            return this;
        }

        internal IssueArticleAssert ShouldHaveNotNextLink()
        {
            _article.Link("next").Should().BeNull();
            return this;
        }

        internal IssueArticleAssert ShouldHaveNextLink(int sequenceNumber)
        {
            _article.Link("next")
                    .ShouldBeGet()
                    .EndingWith($"libraries/{_libraryId}/periodicals/{_issue.PeriodicalId}/volumes/{_issue.VolumeNumber}/issues/{_issue.IssueNumber}/articles/{sequenceNumber}");
            return this;
        }

        internal IssueArticleAssert ShouldHaveNotPreviousLink()
        {
            _article.Link("previous").Should().BeNull();
            return this;
        }

        internal IssueArticleAssert ShouldHavePreviousLink(int sequenceNumber)
        {
            _article.Link("previous")
                    .ShouldBeGet()
                    .EndingWith($"libraries/{_libraryId}/periodicals/{_issue.PeriodicalId}/volumes/{_issue.VolumeNumber}/issues/{_issue.IssueNumber}/articles/{sequenceNumber}");
            return this;
        }

        internal void ShouldMatch(IssueArticleView view)
        {
            _article.Title.Should().Be(view.Title);
            _article.SequenceNumber.Should().Be(view.SequenceNumber);
            _article.WriterAccountId.Should().Be(view.WriterAccountId);
            _article.WriterAccountName.Should().Be(view.WriterAccountName);
            if (view.WriterAssignTimeStamp.HasValue)
            {
                _article.WriterAssignTimeStamp.Should().BeCloseTo(view.WriterAssignTimeStamp.Value, TimeSpan.FromSeconds(2));
            }
            else
            {
                _article.WriterAssignTimeStamp.Should().Be(view.WriterAssignTimeStamp);
            }
            _article.ReviewerAccountId.Should().Be(view.ReviewerAccountId);
            if (view.ReviewerAssignTimeStamp.HasValue)
            {
                _article.ReviewerAssignTimeStamp.Should().BeCloseTo(view.ReviewerAssignTimeStamp.Value, TimeSpan.FromSeconds(2));
            }
            else
            {
                _article.ReviewerAssignTimeStamp.Should().Be(view.ReviewerAssignTimeStamp);
            }
            _article.ReviewerAccountName.Should().Be(view.ReviewerAccountName);
            _article.SeriesName.Should().Be(view.SeriesName);
            _article.SeriesIndex.Should().Be(view.SeriesIndex);
            _article.Status.Should().Be(view.Status);
        }

        internal void ShouldMatch(ArticleDto dto)
        {
            _article.Title.Should().Be(dto.Title);
            _article.SequenceNumber.Should().Be(dto.SequenceNumber);
            _article.WriterAccountId.Should().Be(dto.WriterAccountId);
            if (dto.WriterAssignTimestamp.HasValue)
            {
                _article.WriterAssignTimeStamp.Should().BeCloseTo(dto.WriterAssignTimestamp.Value, TimeSpan.FromSeconds(2));
            }
            else
            {
                _article.WriterAssignTimeStamp.Should().Be(dto.WriterAssignTimestamp);
            }

            _article.ReviewerAccountId.Should().Be(dto.ReviewerAccountId);
            if (dto.ReviewerAssignTimestamp.HasValue)
            {
                _article.ReviewerAssignTimeStamp.Should().BeCloseTo(dto.ReviewerAssignTimestamp.Value, TimeSpan.FromSeconds(2));
            }
            else
            {
                _article.ReviewerAssignTimeStamp.Should().Be(dto.ReviewerAssignTimestamp);

            }

            _article.SeriesName.Should().Be(dto.SeriesName);
            _article.SeriesIndex.Should().Be(dto.SeriesIndex);
            _article.Status.Should().Be(dto.Status.ToString());
        }

        internal IssueArticleAssert ShouldBeSameAs(ArticleDto dto)
        {
            _article.Title.Should().Be(dto.Title);
            _article.SequenceNumber.Should().Be(dto.SequenceNumber);
            _article.WriterAccountId.Should().Be(dto.WriterAccountId);
            _article.WriterAssignTimeStamp.Should().BeCloseTo(dto.WriterAssignTimestamp.Value, TimeSpan.FromSeconds(2));
            _article.ReviewerAccountId.Should().Be(dto.ReviewerAccountId);
            _article.ReviewerAssignTimeStamp.Should().BeCloseTo(dto.ReviewerAssignTimestamp.Value, TimeSpan.FromSeconds(2));
            _article.SeriesName.Should().Be(dto.SeriesName);
            _article.SeriesIndex.Should().Be(dto.SeriesIndex);
            _article.Status.Should().Be(dto.Status.ToString());

            return this;
        }
    }
}
