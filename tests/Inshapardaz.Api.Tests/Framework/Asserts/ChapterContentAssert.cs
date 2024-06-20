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
    public class ChapterContentAssert
    {
        private HttpResponseMessage _response;
        private int _libraryId;
        private ChapterContentView _chapterContent;
        private LibraryDto _library;

        private readonly IChapterTestRepository _chapterRepository;
        private readonly IFileTestRepository _fileRepository;
        private readonly IAuthorTestRepository _authorRepository;
        private readonly ICategoryTestRepository _categoryRepository;
        private readonly ISeriesTestRepository _seriesRepository;
        private readonly FakeFileStorage _fileStorage;

        public ChapterContentAssert(IFileTestRepository fileRepository,
            FakeFileStorage fileStorage,
            IChapterTestRepository chapterRepository)
        {
            _fileRepository = fileRepository;
            _fileStorage = fileStorage;
            _chapterRepository = chapterRepository;
        }

        public ChapterContentAssert ForResponse(HttpResponseMessage response)
        {
            _response = response;
            _chapterContent = response.GetContent<ChapterContentView>().Result;
            return this;
        }

        public ChapterContentAssert ForLibrary(int libraryId)
        {
            _libraryId = libraryId;
            return this;
        }

        public ChapterContentAssert ForLibrary(LibraryDto library)
        {
            _libraryId = library.Id;
            _library = library;
            return this;
        }

        public ChapterContentAssert ShouldHaveSelfLink()
        {
            _chapterContent.SelfLink()
                  .ShouldBeGet()
                  .EndingWith($"libraries/{_libraryId}/books/{_chapterContent.BookId}/chapters/{_chapterContent.ChapterNumber}/contents")
                  .ShouldHaveAcceptLanguage(_chapterContent.Language);

            return this;
        }

        public ChapterContentAssert WithReadOnlyLinks()
        {
            ShouldNotHaveUpdateLink();
            ShouldNotHaveDeleteLink();
            return this;
        }

        public ChapterContentAssert WithWriteableLinks()
        {
            ShouldHaveUpdateLink();
            ShouldHaveDeleteLink();
            return this;
        }

        public ChapterContentAssert ShouldHaveUpdateLink()
        {
            _chapterContent.UpdateLink()
                 .ShouldBePut()
                 .EndingWith($"libraries/{_libraryId}/books/{_chapterContent.BookId}/chapters/{_chapterContent.ChapterNumber}/contents")
                 .ShouldHaveAcceptLanguage(_chapterContent.Language);

            return this;
        }

        public ChapterContentAssert ShouldHaveText(string contents)
        {
            _chapterContent.Text.Should().Be(contents);
            return this;
        }

        public ChapterContentAssert ShouldNotHaveUpdateLink()
        {
            _chapterContent.UpdateLink().Should().BeNull();
            return this;
        }

        public ChapterContentAssert ShouldHaveDefaultLibraryLanguage()
        {
            _chapterContent.Language.Should().Be(_library.Language);
            return this;
        }

        public ChapterContentAssert ShouldHaveCorrectLocationHeader()
        {
            var location = _response.Headers.Location.AbsoluteUri;
            location.Should().NotBeNull();
            location.Should().EndWith($"libraries/{_libraryId}/books/{_chapterContent.BookId}/chapters/{_chapterContent.ChapterNumber}/contents");
            return this;
        }

        public ChapterContentAssert ShouldHaveSavedCorrectText(string expected)
        {
            var content = _chapterRepository.GetChapterContentById(_chapterContent.Id);

            var file = _fileRepository.GetFileById(content.FileId);
            var fileContents = _fileStorage.GetTextFile(file.FilePath, CancellationToken.None).Result;
            fileContents.Should().NotBeNull().And.Be(expected);
            return this;
        }

        public ChapterContentAssert ShouldHaveMatechingTextForLanguage(string expected, string language)
        {
            var content = _chapterRepository.GetChapterContentById(_chapterContent.Id);
            content.Language.Should().Be(language);

            var file = _fileRepository.GetFileById(content.FileId);
            var fileContents = _fileStorage.GetTextFile(file.FilePath, CancellationToken.None).Result;
            fileContents.Should().NotBeNull().And.Be(expected);
            return this;
        }

        public ChapterContentAssert ShouldHaveContentLink()
        {
            _chapterContent.Link("content")
                           .ShouldBeGet();

            return this;
        }

        public ChapterContentAssert ShouldHaveSavedChapterContent()
        {
            var dbContent = _chapterRepository.GetChapterContentById(_chapterContent.Id);
            dbContent.Should().NotBeNull();
            _chapterContent.ChapterId.Should().Be(dbContent.ChapterId);
            _chapterContent.Language.Should().Be(dbContent.Language);

            return this;
        }

        public ChapterContentAssert ShouldHaveDeleteLink()
        {
            _chapterContent.DeleteLink()
                 .ShouldBeDelete()
                 .EndingWith($"libraries/{_libraryId}/books/{_chapterContent.BookId}/chapters/{_chapterContent.ChapterNumber}/contents");

            return this;
        }

        public ChapterContentAssert ShouldNotHaveDeleteLink()
        {
            _chapterContent.DeleteLink().Should().BeNull();
            return this;
        }

        public ChapterContentAssert ShouldHaveChapterLink()
        {
            _chapterContent.Link("chapter")
                .ShouldBeGet()
                .EndingWith($"libraries/{_libraryId}/books/{_chapterContent.BookId}/chapters/{_chapterContent.ChapterNumber}");

            return this;
        }

        public ChapterContentAssert ShouldHaveBookLink()
        {
            _chapterContent.Link("book")
                .ShouldBeGet()
                .EndingWith($"libraries/{_libraryId}/books/{_chapterContent.BookId}");

            return this;
        }

        public ChapterContentAssert ShouldMatch(ChapterContentDto content, int bookId)
        {
            _chapterContent.ChapterId.Should().Be(content.ChapterId);
            _chapterContent.BookId.Should().Be(bookId);
            _chapterContent.Language.Should().Be(content.Language);

            return this;
        }

        public ChapterContentAssert ShouldHaveDeletedContent(ChapterContentDto content)
        {
            var dbContent = _chapterRepository.GetChapterContentById(content.Id);
            dbContent.Should().BeNull("Chapter contnet should be deleted");
            return this;
        }

        public ChapterContentAssert ShouldHaveLocationHeader(RedirectResult result, int bookId, ChapterContentDto content)
        {
            result.Url.Should().NotBeNull();
            result.Url.Should().EndWith($"libraries/{_libraryId}/books/{bookId}/chapters/{content.ChapterId}/contents/{content.Id}");
            return this;
        }
    }
}
