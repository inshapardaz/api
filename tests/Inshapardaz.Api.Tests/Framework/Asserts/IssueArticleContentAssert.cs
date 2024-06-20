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
    public class IssueArticleContentAssert
    {
        private HttpResponseMessage _response;
        private int _libraryId;
        private IssueDto _issue;
        private LibraryDto _library;
        private IssueArticleContentView _articleContent;

        private readonly IIssueTestRepository _issueRepository;
        private readonly IIssueArticleTestRepository _issueArticleRepository;
        private readonly IFileTestRepository _fileRepository;
        private readonly FakeFileStorage _fileStorage;

        public IssueArticleContentAssert(IIssueArticleTestRepository issueArticleRepository, 
            IIssueTestRepository issueRepository, 
            IFileTestRepository fileRepository, 
            FakeFileStorage fileStorage)
        {
            _issueArticleRepository = issueArticleRepository;
            _issueRepository = issueRepository;
            _fileRepository = fileRepository;
            _fileStorage = fileStorage;
        }

        public IssueArticleContentAssert ForResponse(HttpResponseMessage response)
        {
            _response = response;
            _articleContent = response.GetContent<IssueArticleContentView>().Result;
            return this;
        }

        public IssueArticleContentAssert ForLibrary(LibraryDto library)
        {
            _libraryId = library.Id;
            _library = library;
            return this;
        }

        public IssueArticleContentAssert ForIssue(IssueDto issue)
        {
            _issue = issue;
            return this;
        }

        public IssueArticleContentAssert ShouldHaveSelfLink()
        {
            _articleContent.SelfLink()
                  .ShouldBeGet()
                  .EndingWith($"/libraries/{_libraryId}/periodicals/{_articleContent.PeriodicalId}/volumes/{_articleContent.VolumeNumber}/issues/{_articleContent.IssueNumber}/articles/{_articleContent.SequenceNumber}/contents")
                  .ShouldHaveAcceptLanguage(_articleContent.Language);

            return this;
        }

        public IssueArticleContentAssert WithReadOnlyLinks()
        {
            ShouldNotHaveUpdateLink();
            ShouldNotHaveDeleteLink();
            return this;
        }

        public IssueArticleContentAssert WithWriteableLinks()
        {
            ShouldHaveUpdateLink();
            ShouldHaveDeleteLink();
            return this;
        }

        public IssueArticleContentAssert ShouldHaveUpdateLink()
        {
            _articleContent.UpdateLink()
                 .ShouldBePut()
                 .EndingWith($"/libraries/{_libraryId}/periodicals/{_articleContent.PeriodicalId}/volumes/{_articleContent.VolumeNumber}/issues/{_articleContent.IssueNumber}/articles/{_articleContent.SequenceNumber}/contents")
                 .ShouldHaveAcceptLanguage(_articleContent.Language);

            return this;
        }

        public IssueArticleContentAssert ShouldHaveText(string contents)
        {
            _articleContent.Text.Should().Be(contents);
            return this;
        }

        public IssueArticleContentAssert ShouldNotHaveUpdateLink()
        {
            _articleContent.UpdateLink().Should().BeNull();
            return this;
        }

        public IssueArticleContentAssert ShouldHaveDefaultLibraryLanguage()
        {
            _articleContent.Language.Should().Be(_library.Language);
            return this;
        }

        public IssueArticleContentAssert ShouldHaveCorrectLocationHeader()
        {
            var location = _response.Headers.Location.AbsoluteUri;
            location.Should().NotBeNull();
            location.Should().EndWith($"/libraries/{_libraryId}/periodicals/{_articleContent.PeriodicalId}/volumes/{_articleContent.VolumeNumber}/issues/{_articleContent.IssueNumber}/articles/{_articleContent.SequenceNumber}/contents");
            return this;
        }

        public IssueArticleContentAssert ShouldHaveSavedCorrectText(string expected)
        {
            var issueArticle = _issueArticleRepository.GetIssueArticleContent(_issue.PeriodicalId, _issue.VolumeNumber, _issue.IssueNumber, _articleContent.SequenceNumber, _articleContent.Language);
            issueArticle.Should().NotBeNull();
            var file = _fileRepository.GetFileById(issueArticle.FileId.Value);
            file.Should().NotBeNull();
            file.FilePath.Should().Be($"periodicals/{_issue.PeriodicalId}/volumes/{_issue.VolumeNumber}/issues/{_issue.IssueNumber}/articles/{issueArticle.ArticleId}/article-{issueArticle.Language}.md");
            var text = _fileStorage.GetTextFile(file.FilePath, CancellationToken.None).Result;
            text.Should().Be(text);
            return this;
        }

        public IssueArticleContentAssert ShouldHaveMatechingTextForLanguage(string expected, string language)
        {
            var issueArticle = _issueArticleRepository.GetIssueArticleContent(_issue.PeriodicalId, _issue.VolumeNumber, _issue.IssueNumber, _articleContent.SequenceNumber, _articleContent.Language);
            issueArticle.Should().NotBeNull();
            var file = _fileRepository.GetFileById(issueArticle.FileId.Value);
            file.Should().NotBeNull();
            file.FilePath.Should().Be($"periodicals/{_issue.PeriodicalId}/volumes/{_issue.VolumeNumber}/issues/{_issue.IssueNumber}/articles/{issueArticle.ArticleId}/article-{issueArticle.Language}.md");
            var text = _fileStorage.GetTextFile(file.FilePath, CancellationToken.None).Result;
            text.Should().Be(text);
            return this;
        }

        public IssueArticleContentAssert ShouldHaveContentLink()
        {
            _articleContent.Link("contents")
                           .ShouldBeGet();

            return this;
        }

        public IssueArticleContentAssert ShouldHaveSavedArticleContent()
        {
            var dbContent = _issueArticleRepository.GetIssueArticleContent(_issue.PeriodicalId, _issue.VolumeNumber, _issue.IssueNumber, _articleContent.SequenceNumber, _articleContent.Language);
            dbContent.Should().NotBeNull();
            var dbArticle = _issueArticleRepository.GetIssueArticleById(dbContent.ArticleId);
            dbArticle.Should().NotBeNull();
            var dbIssue = _issueRepository.GetIssueById(dbArticle.IssueId);
            _articleContent.PeriodicalId.Should().Be(dbIssue.PeriodicalId);
            _articleContent.VolumeNumber.Should().Be(dbIssue.VolumeNumber);
            _articleContent.IssueNumber.Should().Be(dbIssue.IssueNumber);
            _articleContent.SequenceNumber.Should().Be(dbArticle.SequenceNumber);
            _articleContent.Language.Should().Be(dbContent.Language);

            return this;
        }

        public IssueArticleContentAssert ShouldHaveDeleteLink()
        {
            _articleContent.DeleteLink()
                 .ShouldBeDelete()
                 .EndingWith($"/libraries/{_libraryId}/periodicals/{_articleContent.PeriodicalId}/volumes/{_articleContent.VolumeNumber}/issues/{_articleContent.IssueNumber}/articles/{_articleContent.SequenceNumber}/contents");

            return this;
        }

        public IssueArticleContentAssert ShouldNotHaveDeleteLink()
        {
            _articleContent.DeleteLink().Should().BeNull();
            return this;
        }

        public IssueArticleContentAssert ShouldHaveArticleLink()
        {
            _articleContent.Link("article")
                .ShouldBeGet()
                .EndingWith($"/libraries/{_libraryId}/periodicals/{_articleContent.PeriodicalId}/volumes/{_articleContent.VolumeNumber}/issues/{_articleContent.IssueNumber}/articles/{_articleContent.SequenceNumber}");

            return this;
        }

        public IssueArticleContentAssert ShouldHaveIssueLink()
        {
            _articleContent.Link("issue")
                .ShouldBeGet()
                .EndingWith($"/libraries/{_libraryId}/periodicals/{_articleContent.PeriodicalId}/volumes/{_articleContent.VolumeNumber}/issues/{_articleContent.IssueNumber}");

            return this;
        }

        public IssueArticleContentAssert ShouldHavePeriodicalLink()
        {
            _articleContent.Link("periodical")
                .ShouldBeGet()
                .EndingWith($"/libraries/{_libraryId}/periodicals/{_articleContent.PeriodicalId}");

            return this;
        }

        public IssueArticleContentAssert ShouldMatch(IssueArticleContentDto content, IssueDto issue, IssueArticleDto article)
        {
            _articleContent.PeriodicalId.Should().Be(issue.PeriodicalId);
            _articleContent.VolumeNumber.Should().Be(issue.VolumeNumber);
            _articleContent.IssueNumber.Should().Be(issue.IssueNumber);
            _articleContent.SequenceNumber.Should().Be(article.SequenceNumber);
            _articleContent.Language.Should().Be(content.Language);

            return this;
        }

        public IssueArticleContentAssert ShouldHaveDeletedContent(IssueDto issue, IssueArticleDto article, IssueArticleContentDto content)
        {
            var dbContent = _issueArticleRepository.GetIssueArticleContent(issue.PeriodicalId, issue.VolumeNumber, issue.IssueNumber, article.SequenceNumber, content.Language);
            dbContent.Should().BeNull("Article content should be deleted");
            return this;
        }

        public IssueArticleContentAssert ShouldHaveLocationHeader(RedirectResult result, int libraryId, int periodicalId, int volumeNumber, int issueNumber, ChapterContentDto content)
        {
            result.Url.Should().NotBeNull();
            result.Url.Should().EndWith($"/libraries/{libraryId}/periodicals/{periodicalId}/volumes/{volumeNumber}/issues/{issueNumber}/contents");
            return this;
        }
    }
}
