﻿using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.Dto;
using Inshapardaz.Api.Tests.Helpers;
using Inshapardaz.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.Chapter.Contents.UpdateChapterContents
{
    [TestFixture]
    public class WhenUpdatingChapterContentsAsUnauthorised
        : TestBase
    {
        private HttpResponseMessage _response;
        private ChapterDto _chapter;
        private ChapterContentDto _content;

        private byte[] _newContents;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _chapter = ChapterBuilder.WithLibrary(LibraryId).Public().WithContents().Build();
            _content = ChapterBuilder.Contents.Single(x => x.ChapterId == _chapter.Id);
            var file = ChapterBuilder.Files.Single(x => x.Id == _content.FileId);
            var contents = FileStore.GetFile(file.FilePath, CancellationToken.None).Result;

            _newContents = Random.Bytes;

            _response = await Client.PutContent($"/library/{LibraryId}/books/{_chapter.BookId}/chapters/{_chapter.Id}/contents", _newContents, _content.Language, file.MimeType);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldHaveUnauthorizedResult()
        {
            _response.ShouldBeUnauthorized();
        }
    }
}