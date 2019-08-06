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
    public class WhenUpdatingChapterContentsAsUnauthorised : FunctionTest
    {
        UnauthorizedResult _response;
        FakeFileStorage _fileStore;
        ChapterContent _chapterContents;
        int chapterId;
        string _contents;
        string _newContents;

        ChapterDataBuilder _dataBuilder;

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

            var chapters = _dataBuilder.WithContentLink(contentUrl).WithChapters(1, true, true).Build();
            var chapter = chapters.First();
            _chapterContents = chapter.Contents.FirstOrDefault();
            chapterId = chapter.Id;
            _contents = new Faker().Random.Words(60);
            var handler = Container.GetService<Functions.Library.Books.Chapters.Contents.UpdateChapterContents>();
            var request = new RequestBuilder().WithBody(_contents).Build();
            _response = (UnauthorizedResult) await handler.Run(request, chapter.BookId, chapter.Id, AuthenticationBuilder.Unauthorized, CancellationToken.None);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldHaveUnauthorizedResult()
        {
            Assert.That(_response, Is.Not.Null);
        }

        [Test]
        public async Task ShouldReturnCorrectContentData()
        {
            var _expected = _dataBuilder.GetContentById(chapterId);
            var savedContents = await _fileStore.GetTextFile(_expected.ContentUrl, CancellationToken.None);
            Assert.That(_expected, Is.Not.Null, "Should return chapter");
            Assert.That(_chapterContents.Id, Is.EqualTo(_expected.Id), "Content id does not match");
            Assert.That(_chapterContents.ChapterId, Is.EqualTo(_expected.ChapterId), "Chapter id does not match");
            //Assert.That(savedContents, Is.EqualTo(_contents), "contents should not be updated");
            Assert.That(savedContents, Is.Not.EqualTo(_newContents), "contents should be updated");
        }
    }
}
