﻿using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.Dto;
using Inshapardaz.Api.Tests.Helpers;
using Inshapardaz.Domain.Models;
using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.Chapter.Contents.AddChapterContents
{
    [TestFixture(Role.Admin)]
    [TestFixture(Role.LibraryAdmin)]
    [TestFixture(Role.Writer)]
    public class WhenAddingChapterContentsToPublicBookWithPermission
        : TestBase
    {
        private HttpResponseMessage _response;
        private string _contents;
        private ChapterDto _chapter;
        private ChapterContentAssert _assert;

        public WhenAddingChapterContentsToPublicBookWithPermission(Role role)
            : base(role)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            _chapter = ChapterBuilder.WithLibrary(LibraryId).Public().Build();
            _contents = RandomData.String;

            _response = await Client.PostString($"/libraries/{LibraryId}/books/{_chapter.BookId}/chapters/{_chapter.ChapterNumber}/contents?language={RandomData.Locale}", _contents, RandomData.Locale);
            _assert = new ChapterContentAssert(_response, LibraryId);
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
            _assert.ShouldHaveCorrectLoactionHeader();
        }

        [Test]
        public void ShouldSaveTheChapterContent()
        {
            _assert.ShouldHaveSavedChapterContent(DatabaseConnection);
        }

        [Test]
        public void ShouldHaveTextReturened()
        {
            _assert.ShouldHaveText(_contents);
        }

        [Test]
        public void ShouldHaveCorrectTextSaved()
        {
            _assert.ShouldHaveSavedCorrectText(_contents, DatabaseConnection);
        }

        [Test]
        public void ShouldHaveLinks()
        {
            _assert.ShouldHaveSelfLink()
                   .ShouldHaveBookLink()
                   .ShouldHaveChapterLink()
                   .ShouldHaveContentLink()
                   .ShouldHaveUpdateLink()
                   .ShouldHaveDeleteLink();
        }
    }
}
