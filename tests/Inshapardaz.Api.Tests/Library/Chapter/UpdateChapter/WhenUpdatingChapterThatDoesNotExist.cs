﻿using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.Chapter.UpdateChapter
{
    [TestFixture]
    public class WhenUpdatingChapterThatDoesNotExist
        : TestBase
    {
        private HttpResponseMessage _response;
        private ChapterAssert _chapterAssert;
        private ChapterView _newChapter;

        public WhenUpdatingChapterThatDoesNotExist()
            : base(Role.Writer)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var book = BookBuilder.WithLibrary(LibraryId).Build();

            _newChapter = new ChapterView { Title = RandomData.Name, BookId = book.Id, ChapterNumber = RandomData.Number };

            _response = await Client.PutObject($"/libraries/{LibraryId}/books/{book.Id}/chapters/{_newChapter.ChapterNumber}", _newChapter);

            _chapterAssert = Services.GetService<ChapterAssert>().ForResponse(_response).ForLibrary(LibraryId);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldHaveCreatedResult()
        {
            _response.ShouldBeCreated();
        }

        [Test]
        public void ShouldHaveLocationHeader()
        {
            _chapterAssert.ShouldHaveCorrectLocationHeader();
        }

        [Test]
        public void ShouldSaveTheChapter()
        {
            _chapterAssert.ShouldHaveSavedChapter();
        }

        [Test]
        public void ShouldHaveCorrectObjectReturned()
        {
            _newChapter.Status = "Available";
            _newChapter.ChapterNumber = 1;
            _chapterAssert.ShouldMatch(_newChapter);
        }

        [Test]
        public void ShouldHaveLinks()
        {
            _chapterAssert.ShouldHaveSelfLink()
                   .ShouldHaveBookLink()
                   .ShouldHaveUpdateLink()
                   .ShouldHaveDeleteLink()
                   .ShouldNotHaveContentsLink()
                   .ShouldHaveAddChapterContentLink();
        }
    }
}
