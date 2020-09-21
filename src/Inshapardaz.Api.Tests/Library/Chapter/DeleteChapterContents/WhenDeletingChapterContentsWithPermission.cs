﻿using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.Dto;
using Inshapardaz.Api.Tests.Helpers;
using Inshapardaz.Domain.Adapters;
using NUnit.Framework;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.Chapter.Contents.DeleteChapterContents
{
    [TestFixture(Permission.Admin)]
    [TestFixture(Permission.LibraryAdmin)]
    [TestFixture(Permission.Writer)]
    public class WhenDeletingChapterContentsWithPermission
        : TestBase
    {
        private HttpResponseMessage _response;
        private ChapterContentDto _content;

        public WhenDeletingChapterContentsWithPermission(Permission permission)
            : base(permission)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var _newContents = Random.Words(12);
            var chapter = ChapterBuilder.WithLibrary(LibraryId).WithContents().Build();
            _content = ChapterBuilder.Contents.Single(x => x.ChapterId == chapter.Id);
            var file = ChapterBuilder.Files.Single(x => x.Id == _content.FileId);

            _response = await Client.DeleteAsync($"/library/{LibraryId}/books/{chapter.BookId}/chapters/{chapter.Id}/contents", _content.Language, file.MimeType);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldHaveNoContentResult()
        {
            _response.ShouldBeNoContent();
        }

        [Test]
        public void ShouldHaveDeletedChapterContent()
        {
            ChapterContentAssert.ShouldHaveDeletedContent(DatabaseConnection, _content);
        }
    }
}