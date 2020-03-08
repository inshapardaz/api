﻿using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Bogus;
using Inshapardaz.Domain.Repositories;
using Inshapardaz.Functions.Tests.DataBuilders;
using Inshapardaz.Functions.Tests.DataHelpers;
using Inshapardaz.Functions.Tests.Dto;
using Inshapardaz.Functions.Tests.Fakes;
using Inshapardaz.Functions.Tests.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Functions.Tests.Library.Chapter.Contents.DeleteChapterContents
{
    [TestFixture]
    public class WhenDeletingChapterContentsAsAdministrator : LibraryTest<Functions.Library.Books.Chapters.Contents.DeleteChapterContents>
    {
        private NoContentResult _response;
        private ChapterContentDto _chapterContent;
        private int _contentId;
        private string _contentUrl;
        private ChapterDataBuilder _dataBuilder;

        private FakeFileStorage _fileStore;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _dataBuilder = Container.GetService<ChapterDataBuilder>();

            _fileStore = Container.GetService<IFileStorage>() as FakeFileStorage;

            var faker = new Faker();

            var contentLink = faker.Internet.Url();
            _fileStore.SetupFileContents(contentLink, faker.Random.Words(10));

            var chapter = _dataBuilder.WithContentLink(contentLink).WithContents().AsPublic().Build();
            _chapterContent = DatabaseConnection.GetContentByChapter(chapter.Id).PickRandom();
            _contentId = _chapterContent.Id;
            _contentUrl = _chapterContent.ContentUrl;
            var request = new RequestBuilder().WithAcceptType("text/markdown").Build();
            _response = (NoContentResult)await handler.Run(request, LibraryId, chapter.BookId, _chapterContent.ChapterId, AuthenticationBuilder.AdminClaim, CancellationToken.None);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldHaveNoContentResult()
        {
            Assert.That(_response, Is.Not.Null);
        }

        [Test]
        public void ShouldHaveDeletedChapterContent()
        {
            var expected = DatabaseConnection.GetContentById(_contentId);
            Assert.That(expected, Is.Null, "Chapter contents not deleted");
        }

        [Test]
        public void ShouldHaveDeletedChapterData()
        {
            Assert.That(_fileStore.DoesFileExists(_contentUrl), Is.False, "Chapter data not deleted");
        }
    }
}
