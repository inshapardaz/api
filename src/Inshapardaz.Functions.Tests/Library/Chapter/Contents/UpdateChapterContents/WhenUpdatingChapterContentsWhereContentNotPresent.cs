using System.Linq;
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
        CreatedResult _response;
        int chapterId;
        string _contents;
        ChapterDataBuilder _dataBuilder;

        ChapterContentView _responseBody;

        FakeFileStorage fileStore;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _dataBuilder = Container.GetService<ChapterDataBuilder>();

            fileStore = Container.GetService<IFileStorage>() as FakeFileStorage;

            var faker = new Faker();
            var contentUrl = faker.Internet.Url();
            _contents = faker.Random.Words(12);

            var chapters = _dataBuilder.WithChapters(1, true).Build();
            var _chapter = chapters.First();
            chapterId = _chapter.Id;
            var handler = Container.GetService<Functions.Library.Books.Chapters.Contents.UpdateChapterContents>();
            var request = new RequestBuilder().WithBody(_contents).Build();
            _response = (CreatedResult) await handler.Run(request, _chapter.BookId, chapterId, AuthenticationBuilder.WriterClaim, CancellationToken.None);

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
            var savedContent = await fileStore.GetTextFile(actual.ContentUrl, CancellationToken.None);
            Assert.That(actual, Is.Not.Null, "Should return chapter");
            Assert.That(savedContent, Is.EqualTo(_contents), "contents should be saved");
        }
    }
}
