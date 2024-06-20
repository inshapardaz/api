using FluentAssertions;
using Inshapardaz.Api.Tests.Framework.DataHelpers;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Fakes;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Api.Views.Library;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Threading;

namespace Inshapardaz.Api.Tests.Framework.Asserts
{
    public class ArticleContentAssert
    {
        private HttpResponseMessage _response;
        private int _libraryId;
        private ArticleContentView _articleContent;
        private LibraryDto _library;

        private readonly IArticleTestRepository _articleRepository;
        private readonly IFileTestRepository _fileRepository;
        private readonly FakeFileStorage _fileStorage;
        private ArticleView _article;

        public ArticleContentAssert(IArticleTestRepository articleRepository,
            IFileTestRepository fileRepository,            
            FakeFileStorage fileStorage)
        {
            _articleRepository = articleRepository;
            _fileRepository = fileRepository;
            _fileStorage = fileStorage;
        }

        public ArticleContentAssert ForArticleView(ArticleView view)
        {
            _article = view;
            return this;
        }

        public ArticleContentAssert ForLibrary(int libraryId)
        {
            _libraryId = libraryId;
            return this;
        }

        public ArticleContentAssert ForLibrary(LibraryDto library)
        {
            _libraryId = library.Id;
            _library = library;
            return this;
        }

        public ArticleContentAssert ForResponse(HttpResponseMessage response)
        {
            _response = response;
            _articleContent = response.GetContent<ArticleContentView>().Result;
            return this;
        }

        public ArticleContentAssert ShouldHaveSelfLink()
        {
            _articleContent.SelfLink()
                  .ShouldBeGet()
                  .EndingWith($"/libraries/{_libraryId}/articles/{_articleContent.ArticleId}/contents")
                  .ShouldHaveAcceptLanguage(_articleContent.Language);

            return this;
        }

        public ArticleContentAssert WithReadOnlyLinks()
        {
            ShouldNotHaveUpdateLink();
            ShouldNotHaveDeleteLink();
            return this;
        }

        public ArticleContentAssert WithWriteableLinks()
        {
            ShouldHaveUpdateLink();
            ShouldHaveDeleteLink();
            return this;
        }

        public ArticleContentAssert ShouldHaveUpdateLink()
        {
            _articleContent.UpdateLink()
                 .ShouldBePut()
                 .EndingWith($"/libraries/{_libraryId}/articles/{_articleContent.ArticleId}/contents")
                 .ShouldHaveAcceptLanguage(_articleContent.Language);

            return this;
        }

        public ArticleContentAssert ShouldHaveText(string contents)
        {
            _articleContent.Text.Should().Be(contents);
            return this;
        }


        public ArticleContentAssert ShouldHaveText(ArticleContentDto contents)
        {
            var file = _fileRepository.GetFileById(contents.FileId.Value);
            var fileContents = _fileStorage.GetTextFile(file.FilePath, CancellationToken.None).Result;
            _articleContent.Text.Should().Be(fileContents);
            return this;
        }

        public ArticleContentAssert ShouldNotHaveUpdateLink()
        {
            _articleContent.UpdateLink().Should().BeNull();
            return this;
        }

        public ArticleContentAssert ShouldHaveDefaultLibraryLanguage()
        {
             _articleContent.Language.Should().Be(_library.Language);
            return this;
        }

        public ArticleContentAssert ShouldHaveCorrectLocationHeader(string language)
        {
            var location = _response.Headers.Location.AbsoluteUri;
            location.Should().NotBeNull();
            location.Should().EndWith($"/libraries/{_libraryId}/articles/{_articleContent.ArticleId}/contents?language={language}");
            return this;
        }

        public ArticleContentAssert ShouldHaveSavedCorrectText(string expected)
        {
            var content = _articleRepository.GetArticleContent(_articleContent.ArticleId, _articleContent.Language);
            var file = _fileRepository.GetFileById(content.FileId.Value);
            var fileContents = _fileStorage.GetTextFile(file.FilePath, CancellationToken.None).Result;
            fileContents.Should().Be(expected);
            return this;
        }

        public ArticleContentAssert ShouldHaveMatechingTextForLanguage(string expected, string language, string newLayout)
        {
            var content = _articleRepository.GetArticleContent(_articleContent.ArticleId, _articleContent.Language);
            //TODO: Assert text from file
            //content.Text.Should().NotBeNull().Should().NotBe(expected);
            content.Language.Should().Be(language);
            content.Layout.Should().Be(newLayout);
            return this;
        }

        public ArticleContentAssert ShouldHaveSavedArticleContent()
        {
            var dbContent = _articleRepository.GetArticleContent(_articleContent.ArticleId, _articleContent.Language);
            dbContent.Should().NotBeNull();
            var dbArticle = _articleRepository.GetArticleById(dbContent.ArticleId);
            dbArticle.Should().NotBeNull();
            _articleContent.ArticleId.Should().Be(dbContent.ArticleId);
            _articleContent.Language.Should().Be(dbContent.Language);
            _articleContent.Layout.Should().Be(dbContent.Layout);

            var file = _fileRepository.GetFileById(dbContent.FileId.Value);
            file.Should().NotBeNull();
            file.FilePath.Should().Be($"articles/{dbContent.ArticleId}/article-{dbContent.Language}.md");
            var text = _fileStorage.GetTextFile(file.FilePath, CancellationToken.None).Result;
            text.Should().Be(text);
            return this;
        }

        public ArticleContentAssert ShouldHaveDeleteLink()
        {
            _articleContent.DeleteLink()
                 .ShouldBeDelete()
                 .EndingWith($"/libraries/{_libraryId}/articles/{_articleContent.ArticleId}/contents");

            return this;
        }

        public ArticleContentAssert ShouldNotHaveDeleteLink()
        {
            _articleContent.DeleteLink().Should().BeNull();
            return this;
        }

        public ArticleContentAssert ShouldHaveArticleLink()
        {
            _articleContent.Link("article")
                .ShouldBeGet()
                .EndingWith($"/libraries/{_libraryId}/articles/{_articleContent.ArticleId}");

            return this;
        }

        public ArticleContentAssert ShouldMatch(ArticleContentDto content, ArticleDto article)
        {
            _articleContent.ArticleId.Should().Be(article.Id);
            _articleContent.Language.Should().Be(content.Language);
            _articleContent.Layout.Should().Be(content.Layout);

            return this;
        }

        public ArticleContentAssert ShouldHaveDeletedContent(ArticleContentDto content)
        {
            var dbContent = _articleRepository.GetArticleContent(content.ArticleId, content.Language);
            dbContent.Should().BeNull("Article content should be deleted");
            return this;
        }

        public ArticleContentAssert ShouldHaveContent(long articleId, string language)
        {
            var dbContent = _articleRepository.GetArticleContent(articleId, language);
            dbContent.Should().NotBeNull("Article content should exist.");
            return this;
        }

        public ArticleContentAssert ShouldHaveLocationHeader(RedirectResult result, int libraryId, ArticleContentDto content)
        {
            var response = result as RedirectResult;
            response.Url.Should().NotBeNull();
            response.Url.Should().EndWith($"/libraries/{libraryId}/articles/{content.ArticleId}/contents");
            return this;
        }
    }
}
