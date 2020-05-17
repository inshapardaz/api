using System.Threading;
using System.Threading.Tasks;
using Bogus;
using Inshapardaz.Domain.Repositories;
using Inshapardaz.Functions.Tests.DataBuilders;
using Inshapardaz.Functions.Tests.DataHelpers;
using Inshapardaz.Functions.Tests.Fakes;
using Inshapardaz.Functions.Tests.Helpers;
using Inshapardaz.Functions.Views.Library;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Functions.Tests.Library.Chapter.Contents.UpdateChapterContents
{
    [TestFixture, Ignore("ToFix")]
    public class WhenUpdatingChapterContentsWhereContentNotPresent
        : LibraryTest<Functions.Library.Books.Chapters.Contents.UpdateChapterContents>
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
            _contents = faker.Random.Words(12);

            var chapter = _dataBuilder.WithContents().Build();
            _chapterId = chapter.Id;
            var request = new RequestBuilder().WithBody(_contents).Build();
            _response = (CreatedResult)await handler.Run(request, LibraryId, chapter.BookId, _chapterId, AuthenticationBuilder.WriterClaim, CancellationToken.None);

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
            //var actual = DatabaseConnection.GetContentById(_responseBody.Id);
            //var savedContent = await _fileStore.GetTextFile(actual.ContentUrl, CancellationToken.None);
            //Assert.That(actual, Is.Not.Null, "Should return chapter");
            //Assert.That(savedContent, Is.EqualTo(_contents), "contents should be saved");
        }
    }
}
