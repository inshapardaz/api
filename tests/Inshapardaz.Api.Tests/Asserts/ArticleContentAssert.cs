using FluentAssertions;
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
        private ArticleContentView _articleContent;
        private LibraryDto _library;

        public ArticleContentAssert(HttpResponseMessage response, int libraryId)
        {
            _response = response;
            _libraryId = libraryId;
            _articleContent = response.GetContent<ArticleContentView>().Result;
        }

        public ArticleContentAssert(HttpResponseMessage response, LibraryDto library)
        {
            _response = response;
            _libraryId = library.Id;
            _library = library;
            _articleContent = response.GetContent<ArticleContentView>().Result;
        }

        internal ArticleContentAssert ShouldHaveSelfLink()
        {
            _articleContent.SelfLink()
                  .ShouldBeGet()
                  .EndingWith($"/libraries/{_libraryId}/articles/{_articleContent.ArticleId}/contents")
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
                 .EndingWith($"/libraries/{_libraryId}/articles/{_articleContent.ArticleId}/contents")
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

        internal ArticleContentAssert ShouldHaveCorrectLocationHeader(string language)
        {
            var location = _response.Headers.Location.AbsoluteUri;
            location.Should().NotBeNull();
            location.Should().EndWith($"/libraries/{_libraryId}/articles/{_articleContent.ArticleId}/contents?language={language}");
            return this;
        }

        internal ArticleContentAssert ShouldHaveSavedCorrectText(string expected, IDbConnection dbConnection)
        {
            var content = dbConnection.GetArticleContent(_articleContent.ArticleId, _articleContent.Language);
            content.Text.Should().NotBeNull().And.Be(expected);
            return this;
        }

        internal ArticleContentAssert ShouldHaveMatechingTextForLanguage(string expected, string language, string newLayout, IDbConnection dbConnection)
        {
            var content = dbConnection.GetArticleContent(_articleContent.ArticleId, _articleContent.Language);
            content.Text.Should().NotBeNull().Should().NotBe(expected);
            content.Language.Should().Be(language);
            content.Layout.Should().Be(newLayout);
            return this;
        }

        internal ArticleContentAssert ShouldHaveSavedArticleContent(IDbConnection dbConnection)
        {
            var dbContent = dbConnection.GetArticleContent(_articleContent.ArticleId, _articleContent.Language);
            dbContent.Should().NotBeNull();
            var dbArticle = dbConnection.GetArticleById(dbContent.ArticleId);
            dbArticle.Should().NotBeNull();
            _articleContent.ArticleId.Should().Be(dbContent.ArticleId);
            _articleContent.Language.Should().Be(dbContent.Language);
            _articleContent.Layout.Should().Be(dbContent.Layout);
            _articleContent.Text.Should().Be(dbContent.Text);

            return this;
        }

        internal ArticleContentAssert ShouldHaveDeleteLink()
        {
            _articleContent.DeleteLink()
                 .ShouldBeDelete()
                 .EndingWith($"/libraries/{_libraryId}/articles/{_articleContent.ArticleId}/contents");

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
                .EndingWith($"/libraries/{_libraryId}/articles/{_articleContent.ArticleId}");

            return this;
        }

        internal ArticleContentAssert ShouldMatch(ArticleContentDto content, ArticleDto article)
        {
            _articleContent.ArticleId.Should().Be(article.Id);
            _articleContent.Language.Should().Be(content.Language);
            _articleContent.Layout.Should().Be(content.Layout);
            _articleContent.Text.Should().Be(content.Text);

            return this;
        }

        internal static void ShouldHaveDeletedContent(IDbConnection dbConnection, ArticleContentDto content)
        {
            var dbContent = dbConnection.GetArticleContent(content.ArticleId, content.Language);
            dbContent.Should().BeNull("Article content should be deleted");
        }

        internal static void ShouldHaveContent(IDbConnection dbConnection, long articleId, string language)
        {
            var dbContent = dbConnection.GetArticleContent(articleId, language);
            dbContent.Should().NotBeNull("Article content should exist.");
        }

        internal static void ShouldHaveLocationHeader(RedirectResult result, int libraryId, ArticleContentDto content)
        {
            var response = result as RedirectResult;
            response.Url.Should().NotBeNull();
            response.Url.Should().EndWith($"/libraries/{libraryId}/articles/{content.ArticleId}/contents");
        }
    }
}
