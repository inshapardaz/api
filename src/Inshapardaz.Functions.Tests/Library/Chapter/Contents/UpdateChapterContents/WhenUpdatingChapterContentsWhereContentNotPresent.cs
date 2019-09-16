﻿using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Bogus;
using Inshapardaz.Domain.Repositories;
using Inshapardaz.Functions.Tests.DataBuilders;
using Inshapardaz.Functions.Tests.Fakes;
using Inshapardaz.Functions.Tests.Helpers;
using Inshapardaz.Functions.Views.Library;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Functions.Tests.Library.Chapter.Contents.UpdateChapterContents
{
    [TestFixture]
    public class WhenUpdatingChapterContentsWhereContentNotPresent : FunctionTest
    {
        private CreatedResult _response;
        private int _chapterId;
        private string _contents;
        private ChapterDataBuilder _dataBuilder;

        private ChapterContentView _responseBody;

        private FakeFileStorage _fileStore;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _dataBuilder = Container.GetService<ChapterDataBuilder>();

            _fileStore = Container.GetService<IFileStorage>() as FakeFileStorage;

            var faker = new Faker();
            var contentUrl = faker.Internet.Url();
            _contents = faker.Random.Words(12);

            var chapter = _dataBuilder.WithContents().Build();
            _chapterId = chapter.Id;
            var handler = Container.GetService<Functions.Library.Books.Chapters.Contents.UpdateChapterContents>();
            var request = new RequestBuilder().WithBody(_contents).Build();
            _response = (CreatedResult) await handler.Run(request, chapter.BookId, _chapterId, AuthenticationBuilder.WriterClaim, CancellationToken.None);

            _responseBody = _response.Value as ChapterContentView;
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldHaveCreatedResult()
        {
            Assert.That(_response, Is.Not.Null);
        }

        [Test]
        public async Task ShouldReturnCorrectChapterData()
        {
            var actual = _dataBuilder.GetContentById(_responseBody.Id);
            var savedContent = await _fileStore.GetTextFile(actual.ContentUrl, CancellationToken.None);
            Assert.That(actual, Is.Not.Null, "Should return chapter");
            Assert.That(savedContent, Is.EqualTo(_contents), "contents should be saved");
        }
    }
}