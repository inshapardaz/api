﻿using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Repositories;
using Inshapardaz.Functions.Tests.Asserts;
using Inshapardaz.Functions.Tests.DataBuilders;
using Inshapardaz.Functions.Tests.Dto;
using Inshapardaz.Functions.Tests.Fakes;
using Inshapardaz.Functions.Tests.Helpers;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Functions.Tests.Library.Chapter.Contents.GetChapterContents
{
    [TestFixture(AuthenticationLevel.Administrator)]
    [TestFixture(AuthenticationLevel.Writer)]
    public class WhenGettingPrivateChapterContentWithPermission
        : LibraryTest<Functions.Library.Books.Chapters.Contents.GetChapterContents>
    {
        private ObjectResult _response;
        private ChapterContentAssert _assert;
        private DefaultHttpRequest _request;
        private ChapterDto _chapter;
        private ChapterContentDto _content;
        private ClaimsPrincipal _claim;

        public WhenGettingPrivateChapterContentWithPermission(AuthenticationLevel authenticationLevel)
        {
            _claim = AuthenticationBuilder.CreateClaim(authenticationLevel);
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var dataBuilder = Container.GetService<ChapterDataBuilder>();
            _chapter = dataBuilder.WithLibrary(LibraryId).Private().WithContents().Build();
            _content = dataBuilder.Contents.Single(x => x.ChapterId == _chapter.Id);
            var file = dataBuilder.Files.Single(x => x.Id == _content.FileId);
            var fileStore = Container.GetService<IFileStorage>() as FakeFileStorage;
            var contents = fileStore.GetFile(file.FilePath, CancellationToken.None).Result;

            _request = new RequestBuilder()
                                .WithAccept(file.MimeType)
                                .WithLanguage(_content.Language)
                                .Build();

            _response = (ObjectResult)await handler.Run(_request, LibraryId, _chapter.BookId, _chapter.Id, _claim, CancellationToken.None);

            _assert = new ChapterContentAssert(_response, LibraryId);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldReturnOk()
        {
            _response.ShouldBeOk();
        }

        [Test]
        public void ShouldHaveSelfLink()
        {
            _assert.ShouldHaveSelfLink();
        }

        [Test]
        public void ShouldHaveBookLink()
        {
            _assert.ShouldHaveBookLink();
        }

        [Test]
        public void ShouldHaveChapterLink()
        {
            _assert.ShouldHaveChapterLink();
        }

        [Test]
        public void ShouldHaveDownloadLink()
        {
            _assert.ShouldHaveDownloadLink();
        }

        [Test]
        public void ShouldHaveEditLinks()
        {
            _assert.ShouldHaveUpdateLink();
            _assert.ShouldHaveDeleteLink();
        }

        [Test]
        public void ShouldReturnCorrectChapterData()
        {
            _assert.ShouldMatch(_content, _chapter.BookId, DatabaseConnection);
        }
    }
}