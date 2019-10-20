using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Bogus;
using Inshapardaz.Domain.Repositories;
using Inshapardaz.Functions.Tests.DataBuilders;
using Inshapardaz.Functions.Tests.Fakes;
using Inshapardaz.Functions.Tests.Helpers;
using Inshapardaz.Ports.Database.Entities.Library;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Functions.Tests.Library.Chapter.Contents.UpdateChapterContents
{
    [TestFixture]
    public class WhenUpdatingChapterContentsAsReader : FunctionTest
    {
        private ForbidResult _response;
        private FakeFileStorage _fileStore;
        private ChapterContent _chapterContents;
        private int chapterId;
        private string _contents;
        private string _newContents;

        private ChapterDataBuilder _dataBuilder;

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

            _chapterContents = chapter.Contents.FirstOrDefault();
            chapterId = chapter.Id;
            _contents = new Faker().Random.Words(60);
            var handler = Container.GetService<Functions.Library.Books.Chapters.Contents.UpdateChapterContents>();
            var request = new RequestBuilder().WithBody(_newContents).Build();
            _response = (ForbidResult) await handler.Run(request, chapter.BookId, chapter.Id, AuthenticationBuilder.ReaderClaim, CancellationToken.None);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldHaveForbiddenResult()
        {
            Assert.That(_response, Is.Not.Null);
        }

        [Test]
        public async Task ShouldReturnCorrectContentData()
        {
            var expected = _dataBuilder.GetContentById(chapterId);
            var savedContents = await _fileStore.GetTextFile(expected.ContentUrl, CancellationToken.None);
            Assert.That(expected, Is.Not.Null, "Should return chapter");
            Assert.That(_chapterContents.Id, Is.EqualTo(expected.Id), "Content id does not match");
            Assert.That(_chapterContents.ChapterId, Is.EqualTo(expected.ChapterId), "Chapter id does not match");
            //Assert.That(savedContents, Is.EqualTo(_contents), "contents should not be updated");
            Assert.That(savedContents, Is.Not.EqualTo(_newContents), "contents should be updated");
        }
    }
}
