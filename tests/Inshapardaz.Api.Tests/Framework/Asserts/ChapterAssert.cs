using FluentAssertions;
using Inshapardaz.Api.Extensions;
using Inshapardaz.Api.Tests.Framework.DataHelpers;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Fakes;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Api.Views.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace Inshapardaz.Api.Tests.Framework.Asserts
{
    public class ChapterAssert
    {
        private HttpResponseMessage _response;
        private int _libraryId;
        private ChapterView _chapter;

        private readonly IChapterTestRepository _chapterRepository;
        private readonly FakeFileStorage _fileStorage;

        public ChapterAssert(IChapterTestRepository chapterRepository, FakeFileStorage fileStorage)
        {
            _fileStorage = fileStorage;
            _chapterRepository = chapterRepository;
        }

        public ChapterAssert ForView(ChapterView view)
        {
            _chapter = view;
            return this;
        }

        public ChapterAssert ForResponse(HttpResponseMessage response)
        {
            _response = response;
            _chapter = response.GetContent<ChapterView>().Result;
            return this;
        }

        public ChapterAssert ForLibrary(int libraryId)
        {
            _libraryId = libraryId;
            return this;
        }

        public ChapterAssert ShouldHaveCorrectLocationHeader()
        {
            var location = _response.Headers.Location.AbsoluteUri;
            location.Should().NotBeNull();
            location.Should().EndWith($"libraries/{_libraryId}/books/{_chapter.BookId}/chapters/{_chapter.ChapterNumber}");
            return this;
        }

        public ChapterAssert ShouldBeAssignedToUserForWriting(AccountDto account)
        {
            _chapter.WriterAccountId.Should().Be(account.Id);
            _chapter.WriterAccountName.Should().Be(account.Name);
            _chapter.WriterAssignTimeStamp.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(2));
            return this;
        }

        public ChapterAssert ShouldNotBeAssignedForWriting()
        {
            _chapter.WriterAccountId.Should().BeNull();
            _chapter.WriterAccountName.Should().BeNull();
            _chapter.WriterAssignTimeStamp.Should().BeNull();
            return this;
        }

        public ChapterAssert ShouldBeSavedAssignmentForWriting(AccountDto account)
        {
            var dbChapter = _chapterRepository.GetChapterByBookAndChapter(_chapter.BookId, _chapter.Id);
            dbChapter.WriterAccountId.Should().Be(account.Id);
            dbChapter.WriterAssignTimeStamp.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(2));
            return this;
        }

        public ChapterAssert ShouldBeSavedNoAssignmentForWriting()
        {
            var dbChapter = _chapterRepository.GetChapterByBookAndChapter(_chapter.BookId, _chapter.Id);
            dbChapter.WriterAccountId.Should().BeNull();
            dbChapter.WriterAssignTimeStamp.Should().BeNull();
            return this;
        }

        public ChapterAssert ShouldBeAssignedToUserForReviewing(AccountDto account)
        {
            _chapter.ReviewerAccountId.Should().Be(account.Id);
            _chapter.ReviewerAccountName.Should().Be(account.Name);
            _chapter.ReviewerAssignTimeStamp.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(2));
            return this;
        }

        public ChapterAssert ShouldNotBeAssignedForReviewing()
        {
            _chapter.ReviewerAccountId.Should().BeNull();
            _chapter.ReviewerAccountName.Should().BeNull();
            _chapter.ReviewerAssignTimeStamp.Should().BeNull();
            return this;
        }

        public ChapterAssert ShouldBeSavedAssignmentForReviewing(AccountDto account)
        {
            var dbChapter = _chapterRepository.GetChapterByBookAndChapter(_chapter.BookId, _chapter.Id);
            dbChapter.ReviewerAccountId.Should().Be(account.Id);
            dbChapter.ReviewerAssignTimeStamp.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(2));
            return this;
        }

        public ChapterAssert ShouldBeSavedNoAssignmentForReviewing()
        {
            var dbChapter = _chapterRepository.GetChapterByBookAndChapter(_chapter.BookId, _chapter.Id);
            dbChapter.ReviewerAccountId.Should().BeNull();
            dbChapter.ReviewerAssignTimeStamp.Should().BeNull();
            return this;
        }

        public ChapterAssert ShouldHaveSavedChapter()
        {
            var dbChapter = _chapterRepository.GetChapterByBookAndChapter(_chapter.BookId, _chapter.Id);
            dbChapter.Should().NotBeNull();
            _chapter.Title.Should().Be(dbChapter.Title);
            _chapter.BookId.Should().Be(dbChapter.BookId);
            _chapter.Status.Should().Be(dbChapter.Status.ToDescription());
            _chapter.ChapterNumber.Should().Be(dbChapter.ChapterNumber);
            _chapter.WriterAccountId.Should().Be(dbChapter.WriterAccountId);
            if (_chapter.WriterAssignTimeStamp.HasValue)
            {
                _chapter.WriterAssignTimeStamp.Should().BeCloseTo(dbChapter.WriterAssignTimeStamp.Value, TimeSpan.FromSeconds(3));
            }
            else
            {
                dbChapter.WriterAssignTimeStamp.Should().BeNull();
            }
            _chapter.ReviewerAccountId.Should().Be(dbChapter.ReviewerAccountId);

            if (_chapter.ReviewerAssignTimeStamp.HasValue)
            {
                _chapter.ReviewerAssignTimeStamp.Should().BeCloseTo(dbChapter.ReviewerAssignTimeStamp.Value, TimeSpan.FromSeconds(3));
            }
            else
            {
                dbChapter.ReviewerAssignTimeStamp.Should().BeNull();
            }
            return this;
        }

        public ChapterAssert ShouldHaveDeletedChapter(int chapterId)
        {
            var chapter = _chapterRepository.GetChapterById(chapterId);
            chapter.Should().BeNull();
            return this;
        }

        public ChapterAssert ThatContentsAreDeletedForChapter(int chapterId, IEnumerable<string> filePaths)
        {
            var contents = _chapterRepository.GetContentByChapter(chapterId);
            contents.Should().BeNullOrEmpty();

            foreach (var filePath in filePaths)
            {
                _fileStorage.DoesFileExists(filePath).Should().BeFalse();
            }
            return this;
        }

        public ChapterAssert ShouldHaveSelfLink()
        {
            _chapter.SelfLink()
                  .ShouldBeGet()
                  .EndingWith($"libraries/{_libraryId}/books/{_chapter.BookId}/chapters/{_chapter.ChapterNumber}");

            return this;
        }

        public ChapterAssert WithReadOnlyLinks()
        {
            ShouldNotHaveAddChapterContentLink();
            ShouldNotHaveUpdateLink();
            ShouldNotHaveDeleteLink();
            ShouldNotHaveAssignmentLink();
            return this;
        }

        public ChapterAssert WithWriteableLinks()
        {
            ShouldHaveAddChapterContentLink();
            ShouldHaveUpdateLink();
            ShouldHaveDeleteLink();
            ShouldHaveAssignmentLink();
            return this;
        }

        public void ShouldHaveContentLink(ChapterContentDto content)
        {
            var actual = _chapter.Contents.Single(x => x.Id == content.Id);
            actual.SelfLink()
                  .ShouldBeGet()
                  .ShouldHaveAcceptLanguage(content.Language);
        }

        public ChapterAssert ShouldHaveNoCorrectContents()
        {
            _chapter.Link("content").Should().BeNull();
            return this;    
        }

        public ChapterAssert ShouldHaveBookLink()
        {
            _chapter.Link("book")
                  .ShouldBeGet()
                  .EndingWith($"libraries/{_libraryId}/books/{_chapter.BookId}");

            return this;
        }

        public ChapterAssert ShouldHaveAssignmentLink()
        {
            _chapter.Link("assign")
                  .ShouldBePost()
                  .EndingWith($"libraries/{_libraryId}/books/{_chapter.BookId}/chapters/{_chapter.ChapterNumber}/assign");
            return this;
        }

        public ChapterAssert ShouldNotHaveAssignmentLink()
        {
            _chapter.Link("assign")
                  .Should().BeNull();
            return this;
        }

        public ChapterAssert ShouldHaveCorrectContents()
        {
            var contents = _chapterRepository.GetContentByChapter(_chapter.Id);

            contents.Should().HaveSameCount(_chapter.Contents);

            foreach (var content in contents)
            {
                ShouldHaveContentLink(content);
            }

            return this;
        }

        public ChapterAssert ShouldHaveUpdateLink()
        {
            _chapter.UpdateLink()
                 .ShouldBePut()
                 .EndingWith($"libraries/{_libraryId}/books/{_chapter.BookId}/chapters/{_chapter.ChapterNumber}");

            return this;
        }

        public ChapterAssert ShouldNotHaveUpdateLink()
        {
            _chapter.UpdateLink().Should().BeNull();
            return this;
        }

        public ChapterAssert ShouldHaveDeleteLink()
        {
            _chapter.DeleteLink()
                 .ShouldBeDelete()
                 .EndingWith($"libraries/{_libraryId}/books/{_chapter.BookId}/chapters/{_chapter.ChapterNumber}");

            return this;
        }

        public ChapterAssert ShouldNotHaveDeleteLink()
        {
            _chapter.DeleteLink().Should().BeNull();
            return this;
        }

        public ChapterAssert ShouldHaveAddChapterContentLink()
        {
            _chapter.Link("add-content")
                 .ShouldBePost()
                 .EndingWith($"libraries/{_libraryId}/books/{_chapter.BookId}/chapters/{_chapter.ChapterNumber}/contents");

            return this;
        }

        public ChapterAssert ShouldNotHaveAddChapterContentLink()
        {
            _chapter.Link("add-content").Should().BeNull();
            return this;
        }

        public ChapterAssert ShouldHaveUpdateChapterContentLink(ChapterContentDto content)
        {
            var actual = _chapter.Contents.Single(x => x.Id == content.Id);
            actual.UpdateLink()
                  .ShouldBePut()
                  .EndingWith($"libraries/{_libraryId}/books/{_chapter.BookId}/chapters/{_chapter.ChapterNumber}/contents")
                  .ShouldHaveAcceptLanguage(content.Language);

            return this;
        }

        public ChapterAssert ShouldHaveDeleteChapterContentLink(ChapterContentDto content)
        {
            var actual = _chapter.Contents.Single(x => x.Id == content.Id);
            actual.DeleteLink()
                  .ShouldBeDelete()
                  .EndingWith($"libraries/{_libraryId}/books/{_chapter.BookId}/chapters/{_chapter.ChapterNumber}/contents")
                  .ShouldHaveAcceptLanguage(actual.Language);

            return this;
        }

        public ChapterAssert ShouldNotHaveContentsLink()
        {
            _chapter.Link("content").Should().BeNull();
            return this;
        }

        public ChapterAssert ShouldHaveNotNextLink()
        {
            _chapter.Link("next").Should().BeNull();
            return this;
        }

        public ChapterAssert ShouldHaveNextLink(int chapterNumber)
        {
            _chapter.Link("next")
                    .ShouldBeGet()
                    .EndingWith($"/chapters/{chapterNumber}");
            return this;
        }

        public ChapterAssert ShouldHaveNotPreviousLink()
        {
            _chapter.Link("previous").Should().BeNull();
            return this;
        }

        public ChapterAssert ShouldHavePreviousLink(int chapterNumber)
        {
            _chapter.Link("previous")
                    .ShouldBeGet()
                    .EndingWith($"/chapters/{chapterNumber}");
            return this;
        }

        public void ShouldMatch(ChapterView view)
        {
            _chapter.Title.Should().Be(view.Title);
            _chapter.BookId.Should().Be(view.BookId);
            _chapter.Status.Should().Be(view.Status);
            _chapter.ChapterNumber.Should().Be(view.ChapterNumber);
            _chapter.WriterAccountId.Should().Be(view.WriterAccountId);
            if (_chapter.WriterAssignTimeStamp.HasValue)
            {
                _chapter.WriterAssignTimeStamp.Should().BeCloseTo(view.WriterAssignTimeStamp.Value, TimeSpan.FromSeconds(3));
            }
            else
            {
                view.WriterAssignTimeStamp.Should().BeNull();
            }
            _chapter.ReviewerAccountId.Should().Be(view.ReviewerAccountId);

            if (_chapter.ReviewerAssignTimeStamp.HasValue)
            {
                _chapter.ReviewerAssignTimeStamp.Should().BeCloseTo(view.ReviewerAssignTimeStamp.Value, TimeSpan.FromSeconds(3));
            }
            else
            {
                view.ReviewerAssignTimeStamp.Should().BeNull();
            }
        }

        public void ShouldMatch(ChapterDto dto)
        {
            _chapter.Title.Should().Be(dto.Title);
            _chapter.BookId.Should().Be(dto.BookId);
            _chapter.ChapterNumber.Should().Be(dto.ChapterNumber);
        }

        public ChapterAssert ShouldBeSameAs(ChapterDto dto)
        {
            _chapter.Title.Should().Be(dto.Title);
            _chapter.ChapterNumber.Should().Be(dto.ChapterNumber);
            _chapter.BookId.Should().Be(dto.BookId);

            return this;
        }
    }
}
