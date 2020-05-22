﻿using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Bogus;
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

namespace Inshapardaz.Functions.Tests.Library.Chapter.Contents.AddChapterContents
{
    [TestFixture]
    public class WhenAddingChapterContentsWithoutLanguage
        : LibraryTest<Functions.Library.Books.Chapters.Contents.AddChapterContents>
    {
        private CreatedResult _response;
        private byte[] _contents;
        private DefaultHttpRequest _request;
        private ChapterDataBuilder _dataBuilder;
        private FakeFileStorage _fileStorage;
        private ChapterDto _chapter;
        private ChapterContentAssert _assert;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _dataBuilder = Container.GetService<ChapterDataBuilder>();
            _fileStorage = Container.GetService<IFileStorage>() as FakeFileStorage;

            _chapter = _dataBuilder.WithLibrary(LibraryId).Build();
            _contents = new Faker().Random.Bytes(60);
            _request = new RequestBuilder().WithBytes(_contents).WithoutLanguage().WithContentType("text/plain").Build();
            _response = (CreatedResult)await handler.Run(_request, LibraryId, _chapter.BookId, _chapter.Id, AuthenticationBuilder.WriterClaim, CancellationToken.None);

            _assert = new ChapterContentAssert(_response, Library);
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
        public void ShouldHaveUsedLibraryLanguageForContent()
        {
            _assert.ShouldHaveDefaultLibraryLanguage();
        }
    }
}
