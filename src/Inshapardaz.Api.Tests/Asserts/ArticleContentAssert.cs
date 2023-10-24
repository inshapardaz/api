﻿using FluentAssertions;
using FluentAssertions.Common;
using Inshapardaz.Api.Tests.DataHelpers;
using Inshapardaz.Api.Tests.Dto;
using Inshapardaz.Api.Tests.Helpers;
using Inshapardaz.Api.Views.Library;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Net.Http;

namespace Inshapardaz.Api.Tests.Asserts
{
    internal class ArticleContentAssert
    {
        private HttpResponseMessage _response;
        private readonly int _libraryId;
        private IssueArticleContentView _articleContent;
        private IssueDto _issue;
        private LibraryDto _library;

        public ArticleContentAssert(HttpResponseMessage response, int libraryId, IssueDto issue)
        {
            _response = response;
            _libraryId = libraryId;
            _issue = issue;
            _articleContent = response.GetContent<IssueArticleContentView>().Result;
        }

        public ArticleContentAssert(HttpResponseMessage response, LibraryDto library, IssueDto issue)
        {
            _response = response;
            _libraryId = library.Id;
            _library = library;
            _issue = issue;
            _articleContent = response.GetContent<IssueArticleContentView>().Result;
        }

        internal ArticleContentAssert ShouldHaveSelfLink()
        {
            _articleContent.SelfLink()
                  .ShouldBeGet()
                  .EndingWith($"/libraries/{_libraryId}/periodicals/{_articleContent.PeriodicalId}/volumes/{_articleContent.VolumeNumber}/issues/{_articleContent.IssueNumber}/articles/{_articleContent.SequenceNumber}/contents")
                  .ShouldHaveAcceptLanguage(_articleContent.Language);

            return this;
        }

        internal ArticleContentAssert WithReadOnlyLinks()
        {
            ShouldNotHaveUpdateLink();
            ShouldNotHaveDeleteLink();
            return this;
        }

        internal ArticleContentAssert WithWriteableLinks()
        {
            ShouldHaveUpdateLink();
            ShouldHaveDeleteLink();
            return this;
        }

        internal ArticleContentAssert ShouldHaveUpdateLink()
        {
            _articleContent.UpdateLink()
                 .ShouldBePut()
                 .EndingWith($"/libraries/{_libraryId}/periodicals/{_articleContent.PeriodicalId}/volumes/{_articleContent.VolumeNumber}/issues/{_articleContent.IssueNumber}/articles/{_articleContent.SequenceNumber}/contents")
                 .ShouldHaveAcceptLanguage(_articleContent.Language);

            return this;
        }

        internal ArticleContentAssert ShouldHaveText(string contents)
        {
            _articleContent.Text.Should().Be(contents);
            return this;
        }

        internal ArticleContentAssert ShouldNotHaveUpdateLink()
        {
            _articleContent.UpdateLink().Should().BeNull();
            return this;
        }

        internal ArticleContentAssert ShouldHaveDefaultLibraryLanguage()
        {
            _articleContent.Language.Should().Be(_library.Language);
            return this;
        }

        internal ArticleContentAssert ShouldHaveCorrectLocationHeader()
        {
            var location = _response.Headers.Location.AbsoluteUri;
            location.Should().NotBeNull();
            location.Should().EndWith($"/libraries/{_libraryId}/periodicals/{_articleContent.PeriodicalId}/volumes/{_articleContent.VolumeNumber}/issues/{_articleContent.IssueNumber}/articles/{_articleContent.SequenceNumber}/contents");
            return this;
        }

        internal ArticleContentAssert ShouldHaveSavedCorrectText(string expected, IDbConnection dbConnection)
        {
            var content = dbConnection.GetIssueArticleContent(_issue.PeriodicalId, _issue.VolumeNumber, _issue.IssueNumber, _articleContent.SequenceNumber, _articleContent.Language);
            content.Text.Should().NotBeNull().And.Be(expected);
            return this;
        }

        internal ArticleContentAssert ShouldHaveMatechingTextForLanguage(string expected, string language, IDbConnection dbConnection)
        {
            var content = dbConnection.GetIssueArticleContent(_issue.PeriodicalId, _issue.VolumeNumber, _issue.IssueNumber, _articleContent.SequenceNumber, _articleContent.Language);
            content.Text.Should().NotBeNull().Should().NotBe(expected);
            content.Language.Should().Be(language);
            return this;
        }

        internal ArticleContentAssert ShouldHaveContentLink()
        {
            _articleContent.Link("contents")
                           .ShouldBeGet();

            return this;
        }

        internal ArticleContentAssert ShouldHaveSavedArticleContent(IDbConnection dbConnection)
        {
            var dbContent = dbConnection.GetIssueArticleContent(_issue.PeriodicalId, _issue.VolumeNumber, _issue.IssueNumber, _articleContent.SequenceNumber, _articleContent.Language);
            dbContent.Should().NotBeNull();
            var dbArticle = dbConnection.GetIssueArticleById(dbContent.ArticleId);
            dbArticle.Should().NotBeNull();
            var dbIssue = dbConnection.GetIssueById(dbArticle.IssueId);
            _articleContent.PeriodicalId.Should().Be(dbIssue.PeriodicalId);
            _articleContent.VolumeNumber.Should().Be(dbIssue.VolumeNumber);
            _articleContent.IssueNumber.Should().Be(dbIssue.IssueNumber);
            _articleContent.SequenceNumber.Should().Be(dbArticle.SequenceNumber);
            _articleContent.Language.Should().Be(dbContent.Language);

            return this;
        }

        internal ArticleContentAssert ShouldHaveDeleteLink()
        {
            _articleContent.DeleteLink()
                 .ShouldBeDelete()
                 .EndingWith($"/libraries/{_libraryId}/periodicals/{_articleContent.PeriodicalId}/volumes/{_articleContent.VolumeNumber}/issues/{_articleContent.IssueNumber}/articles/{_articleContent.SequenceNumber}/contents");

            return this;
        }

        internal ArticleContentAssert ShouldNotHaveDeleteLink()
        {
            _articleContent.DeleteLink().Should().BeNull();
            return this;
        }

        internal ArticleContentAssert ShouldHaveArticleLink()
        {
            _articleContent.Link("article")
                .ShouldBeGet()
                .EndingWith($"/libraries/{_libraryId}/periodicals/{_articleContent.PeriodicalId}/volumes/{_articleContent.VolumeNumber}/issues/{_articleContent.IssueNumber}/articles/{_articleContent.SequenceNumber}");

            return this;
        }

        internal ArticleContentAssert ShouldHaveIssueLink()
        {
            _articleContent.Link("issue")
                .ShouldBeGet()
                .EndingWith($"/libraries/{_libraryId}/periodicals/{_articleContent.PeriodicalId}/volumes/{_articleContent.VolumeNumber}/issues/{_articleContent.IssueNumber}");

            return this;
        }

        internal ArticleContentAssert ShouldHavePeriodicalLink()
        {
            _articleContent.Link("periodical")
                .ShouldBeGet()
                .EndingWith($"/libraries/{_libraryId}/periodicals/{_articleContent.PeriodicalId}");

            return this;
        }

        internal ArticleContentAssert ShouldMatch(IssueArticleContentDto content, IssueDto issue, IssueArticleDto article)
        {
            _articleContent.PeriodicalId.Should().Be(issue.PeriodicalId);
            _articleContent.VolumeNumber.Should().Be(issue.VolumeNumber);
            _articleContent.IssueNumber.Should().Be(issue.IssueNumber);
            _articleContent.SequenceNumber.Should().Be(article.SequenceNumber);
            _articleContent.Language.Should().Be(content.Language);

            return this;
        }

        internal static void ShouldHaveDeletedContent(IDbConnection dbConnection, IssueDto issue, IssueArticleDto article, IssueArticleContentDto content)
        {
            var dbContent = dbConnection.GetIssueArticleContent(issue.PeriodicalId, issue.VolumeNumber, issue.IssueNumber, article.SequenceNumber, content.Language);
            dbContent.Should().BeNull("Article content should be deleted");
        }

        internal static void ShouldHaveLocationHeader(RedirectResult result, int libraryId, int periodicalId, int volumeNumber, int issueNumber, ChapterContentDto content)
        {
            var response = result as RedirectResult;
            response.Url.Should().NotBeNull();
            response.Url.Should().EndWith($"/libraries/{libraryId}/periodicals/{periodicalId}/volumes/{volumeNumber}/issues/{issueNumber}/contents");
        }
    }
}
