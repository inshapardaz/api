using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Bogus;
using Inshapardaz.Domain.Repositories;
using Inshapardaz.Functions.Tests.DataBuilders;
using Inshapardaz.Functions.Tests.Fakes;
using Inshapardaz.Functions.Tests.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Functions.Tests.Library.Chapter.Contents.UpdateChapterContents
{
    [TestFixture]
    public class WhenUpdatingChapterContentsAsAdministrator : FunctionTest
    {
        NoContentResult _response;
        Inshapardaz.Ports.Database.Entities.Library.ChapterContent _chapterContent;
        string _contents;
        int chapterId;
        string _newContents;
        ChapterDataBuilder _dataBuilder;

        FakeFileStorage fileStore;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _dataBuilder = Container.GetService<ChapterDataBuilder>();

            fileStore = Container.GetService<IFileStorage>() as FakeFileStorage;

            var faker = new Faker();
            var contentUrl = faker.Internet.Url();
            _contents = faker.Random.Words(10);
            _newContents = faker.Random.Words(12);

            fileStore.SetupFileContents(contentUrl, _contents);

            var chapters = _dataBuilder.WithContentLink(contentUrl).WithChapters(1, true, true).Build();
            var _chapter = chapters.First();
            _contents = new Faker().Random.Words(60);
            _chapterContent = _chapter.Contents.FirstOrDefault();
            chapterId = _chapter.Id;
            var handler = Container.GetService<Functions.Library.Books.Chapters.Contents.UpdateChapterContents>();
            var request = new RequestBuilder().WithBody(_newContents).Build();
            _response = (NoContentResult) await handler.Run(request, _chapter.BookId, chapterId, AuthenticationBuilder.AdminClaim, CancellationToken.None);
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
            var expected = _dataBuilder.GetContentByChapterIdId(chapterId);
            var savedContent = await fileStore.GetTextFile(expected.ContentUrl, CancellationToken.None);
            Assert.That(_chapterContent, Is.Not.Null, "Should return chapter");
            Assert.That(_chapterContent.Id, Is.EqualTo(expected.Id), "Content id does not match");
            Assert.That(_chapterContent.ChapterId, Is.EqualTo(expected.ChapterId), "Chapter id does not match");
            Assert.That(savedContent, Is.Not.EqualTo(_contents), "contents should be updated");
            Assert.That(savedContent, Is.EqualTo(_newContents), "contents should match changes");
        }
    }
}
