﻿using FluentAssertions;
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
    internal class ArticleAssert
    {
        private HttpResponseMessage _response;
        private readonly int _libraryId;
        private ArticleView _article;

        public ArticleAssert(ArticleView view, int libraryId)
        {
            _libraryId = libraryId;
            _article = view;
        }

        public ArticleAssert(HttpResponseMessage response, int libraryId)
        {
            _response = response;
            _libraryId = libraryId;
            _article = response.GetContent<ArticleView>().Result;
        }

        internal static ArticleAssert FromResponse(HttpResponseMessage response, int libraryId)
        {
            return new ArticleAssert(response, libraryId);
        }

        internal static ArticleAssert FromObject(ArticleView view, int libraryId)
        {
            return new ArticleAssert(view, libraryId);
        }

        internal ArticleAssert ShouldHaveCorrectLocationHeader()
        {
            var location = _response.Headers.Location.AbsoluteUri;
            location.Should().NotBeNull();
            location.Should().EndWith($"libraries/{_libraryId}/articles/{_article.Id}");
            return this;
        }

        internal ArticleAssert ShouldBeAssignedToUserForWriting(AccountDto account)
        {
            _article.WriterAccountId.Should().Be(account.Id);
            _article.WriterAccountName.Should().Be(account.Name);
            _article.WriterAssignTimeStamp.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(2));
            return this;
        }

        internal ArticleAssert ShouldNotBeAssignedForWriting()
        {
            _article.WriterAccountId.Should().BeNull();
            _article.WriterAccountName.Should().BeNull();
            _article.WriterAssignTimeStamp.Should().BeNull();
            return this;
        }

        internal ArticleAssert ShouldBeSavedAssignmentForWriting(IDbConnection dbConnection, AccountDto account)
        {
            var dbArticle = dbConnection.GetArticleById(_article.Id);
            dbArticle.WriterAccountId.Should().Be(account.Id);
            dbArticle.WriterAssignTimestamp.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(2));
            return this;
        }

        internal ArticleAssert ShouldBeSavedNoAssignmentForWriting(IDbConnection dbConnection)
        {
            var dbArticle = dbConnection.GetArticleById(_article.Id);
            dbArticle.WriterAccountId.Should().BeNull();
            dbArticle.WriterAssignTimestamp.Should().BeNull();
            return this;
        }

        internal ArticleAssert ShouldBeAssignedToUserForReviewing(AccountDto account)
        {
            _article.ReviewerAccountId.Should().Be(account.Id);
            _article.ReviewerAccountName.Should().Be(account.Name);
            _article.ReviewerAssignTimeStamp.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(2));
            return this;
        }

        internal ArticleAssert ShouldNotBeAssignedForReviewing()
        {
            _article.ReviewerAccountId.Should().BeNull();
            _article.ReviewerAccountName.Should().BeNull();
            _article.ReviewerAssignTimeStamp.Should().BeNull();
            return this;
        }

        internal ArticleAssert ShouldBeSavedAssignmentForReviewing(IDbConnection dbConnection, AccountDto account)
        {
            var dbArticle = dbConnection.GetArticleById(_article.Id);
            dbArticle.ReviewerAccountId.Should().Be(account.Id);
            dbArticle.ReviewerAssignTimestamp.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(2));
            return this;
        }

        internal ArticleAssert ShouldBeSavedNoAssignmentForReviewing(IDbConnection dbConnection)
        {
            var dbArticle = dbConnection.GetArticleById(_article.Id);
            dbArticle.ReviewerAccountId.Should().BeNull();
            dbArticle.ReviewerAssignTimestamp.Should().BeNull();
            return this;
        }

        internal ArticleAssert ShouldHaveSavedArticle(IDbConnection dbConnection)
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

        internal ArticleAssert ShouldHaveSelfLink()
        {
            _article.SelfLink()
                  .ShouldBeGet()
                  .EndingWith($"libraries/{_libraryId}/articles/{_article.Id}");

            return this;
        }

        internal ArticleAssert WithReadOnlyLinks()
        {
            ShouldHaveSelfLink()
            .ShouldNotHaveAddArticleContentLink()
            .ShouldNotHaveUpdateLink()
            .ShouldNotHaveDeleteLink()
            .ShouldNotHaveAssignmentLink();
            return this;
        }

        internal ArticleAssert WithWriteableLinks()
        {
            ShouldHaveAddIssueContentLink()
            .ShouldHaveUpdateLink()
            .ShouldHaveDeleteLink()
            .ShouldHaveAssignmentLink();
            return this;
        }

        internal void ShouldHaveContentLink(ArticleContentDto content)
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

        internal ArticleAssert ShouldHaveAssignmentLink()
        {
            _article.Link("assign")
                  .ShouldBePost()
                  .EndingWith($"libraries/{_libraryId}/articles/{_article.Id}/assign");
            return this;
        }

        internal ArticleAssert ShouldNotHaveAssignmentLink()
        {
            _article.Link("assign")
                  .Should().BeNull();
            return this;
        }

        internal void ShouldHaveCorrectContents(IDbConnection db)
        {
            var contents = db.GetArticleContents(_article.Id);

            contents.Should().HaveSameCount(_article.Contents);

            foreach (var content in contents)
            {
                ShouldHaveContentLink(content);
            }
        }

        internal ArticleAssert ShouldHaveUpdateLink()
        {
            _article.UpdateLink()
                 .ShouldBePut()
                 .EndingWith($"libraries/{_libraryId}/articles/{_article.Id}");

            return this;
        }

        internal ArticleAssert ShouldNotHaveUpdateLink()
        {
            _article.UpdateLink().Should().BeNull();
            return this;
        }

        internal ArticleAssert ShouldHaveDeleteLink()
        {
            _article.DeleteLink()
                 .ShouldBeDelete()
                 .EndingWith($"libraries/{_libraryId}/articles/{_article.Id}");

            return this;
        }

        internal ArticleAssert ShouldNotHaveDeleteLink()
        {
            _article.DeleteLink().Should().BeNull();
            return this;
        }

        internal ArticleAssert ShouldHaveAddIssueContentLink()
        {
            _article.Link("add-content")
                 .ShouldBePost()
                 .EndingWith($"libraries/{_libraryId}/articles/{_article.Id}");

            return this;
        }

        internal ArticleAssert ShouldNotHaveAddArticleContentLink()
        {
            _article.Link("add-content").Should().BeNull();
            return this;
        }

        internal ArticleAssert ShouldHaveUpdateContentLink(IssueContentDto content)
        {
            var actual = _article.Contents.Single(x => x.Id == content.Id);
            actual.UpdateLink()
                  .ShouldBePut()
                  .EndingWith($"libraries/{_libraryId}/articles/{_article.Id}")
                  .ShouldHaveAcceptLanguage(content.Language);

            return this;
        }

        internal ArticleAssert ShouldHaveDeleteContentLink(IssueContentDto content)
        {
            var actual = _article.Contents.Single(x => x.Id == content.Id);
            actual.DeleteLink()
                  .ShouldBeDelete()
                  .EndingWith($"libraries/{_libraryId}/articles/{_article.Id}")
                  .ShouldHaveAcceptLanguage(actual.Language);

            return this;
        }

        internal ArticleAssert ShouldNotHaveContentsLink()
        {
            _article.Link("content").Should().BeNull();
            return this;
        }

        internal void ShouldMatch(ArticleView view)
        {
            _article.Title.Should().Be(view.Title);
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
            _article.Status.Should().Be(view.Status);
        }

        internal void ShouldMatch(ArticleDto dto)
        {
            _article.Title.Should().Be(dto.Title);
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

            _article.Status.Should().Be(dto.Status.ToString());
        }

        internal ArticleAssert ShouldBeSameAs(ArticleDto dto)
        {
            _article.Title.Should().Be(dto.Title);
            _article.WriterAccountId.Should().Be(dto.WriterAccountId);
            _article.WriterAssignTimeStamp.Should().BeCloseTo(dto.WriterAssignTimestamp.Value, TimeSpan.FromSeconds(2));
            _article.ReviewerAccountId.Should().Be(dto.ReviewerAccountId);
            _article.ReviewerAssignTimeStamp.Should().BeCloseTo(dto.ReviewerAssignTimestamp.Value, TimeSpan.FromSeconds(2));
            _article.Status.Should().Be(dto.Status.ToString());

            return this;
        }
    }
}
