using System.Linq;
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

namespace Inshapardaz.Functions.Tests.Library.Chapter.Contents.UpdateChapterContents
{
    [TestFixture]
    public class WhenUpdatingChapterContentsAsWriter
        : LibraryTest<Functions.Library.Books.Chapters.Contents.UpdateChapterContents>
    {
        private NoContentResult _response;
        private ChapterContentDto _chapterContent;
        private string _contents;
        private int _chapterId;
        private string _newContents;
        private ChapterDataBuilder _dataBuilder;

        private FakeFileStorage _fileStore;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _dataBuilder = Container.GetService<ChapterDataBuilder>();

            _fileStore = Container.GetService<IFileStorage>() as FakeFileStorage;

            var faker = new Faker();
            var contentUrl = faker.Internet.Url();
            _contents = faker.Random.Words(10);
            _newContents = faker.Random.Words(12);

            _fileStore.SetupFileContents(contentUrl, _contents);

            var chapter = _dataBuilder.WithContentLink(contentUrl).WithContents().AsPublic().Build();
            _contents = new Faker().Random.Words(60);
            _chapterContent = DatabaseConnection.GetContentByChapter(chapter.Id).PickRandom();
            _chapterId = chapter.Id;

            var request = new RequestBuilder().WithBody(_newContents).Build();
            _response = (NoContentResult)await handler.Run(request, LibraryId, chapter.BookId, _chapterId, AuthenticationBuilder.WriterClaim, CancellationToken.None);
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
        public async Task ShouldReturnCorrectChapterData()
        {
            var expected = DatabaseConnection.GetContentByChapter(_chapterId).FirstOrDefault();
            var savedContent = await _fileStore.GetTextFile(expected.ContentUrl, CancellationToken.None);
            Assert.That(_chapterContent, Is.Not.Null, "Should return chapter");
            Assert.That(_chapterContent.Id, Is.EqualTo(expected.Id), "Content id does not match");
            Assert.That(_chapterContent.ChapterId, Is.EqualTo(expected.ChapterId), "Chapter id does not match");
            Assert.That(savedContent, Is.Not.EqualTo(_contents), "contents should be updated");
            Assert.That(savedContent, Is.EqualTo(_newContents), "contents should match changes");
        }
    }
}
