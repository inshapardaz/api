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

namespace Inshapardaz.Functions.Tests.Library.Chapter.Contents.GetChapterContents
{
    [TestFixture]
    public class WhenGettingChapterContentsAsWriter : LibraryTest<Functions.Library.Books.Chapters.Contents.GetChapterContents>
    {
        private OkObjectResult _response;
        private ChapterContentView _view;

        private string _contents;
        private Ports.Database.Entities.Library.ChapterContent _expected;
        private Ports.Database.Entities.Library.Chapter _chapter;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var dataBuilder = Container.GetService<ChapterDataBuilder>();
            var fileStore = Container.GetService<IFileStorage>() as FakeFileStorage;

            var faker = new Faker();
            var contentUrl = faker.Internet.Url();
            _contents = faker.Random.Words(10);
            fileStore.SetupFileContents(contentUrl, _contents);
            _chapter = dataBuilder.WithContentLink(contentUrl).WithContents().AsPublic().Build();
            _expected = _chapter.Contents.First();

            _response = (OkObjectResult)await handler.Run(null, LibraryId, _chapter.BookId, _chapter.Id, AuthenticationBuilder.WriterClaim, CancellationToken.None);

            _view = _response.Value as ChapterContentView;
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldReturnOk()
        {
            Assert.That(_response, Is.Not.Null);
            Assert.That(_response.StatusCode, Is.EqualTo(200));
        }

        [Test]
        public void ShouldHaveSelfLink()
        {
            _view.Links.AssertLink("self")
                 .ShouldBeGet()
                 .ShouldHaveSomeHref();
        }

        [Test]
        public void ShouldHaveBookLink()
        {
            _view.Links.AssertLink("book")
                 .ShouldBeGet()
                 .ShouldHaveSomeHref();
        }

        [Test]
        public void ShouldHaveChapterLink()
        {
            _view.Links.AssertLink("chapter")
                 .ShouldBeGet()
                 .ShouldHaveSomeHref();
        }

        [Test]
        public void ShouldHaveUpdateLink()
        {
            _view.Links.AssertLink("update")
                 .ShouldBePut()
                 .ShouldHaveSomeHref();
        }

        [Test]
        public void ShouldHaveDeleteLink()
        {
            _view.Links.AssertLink("delete")
                 .ShouldBeDelete()
                 .ShouldHaveSomeHref();
        }

        [Test]
        public void ShouldReturnCorrectChapterData()
        {
            Assert.That(_view, Is.Not.Null, "Should return chapter");
            Assert.That(_view.Id, Is.EqualTo(_expected.Id), "Content id does not match");
            Assert.That(_view.ChapterId, Is.EqualTo(_expected.ChapterId), "Chapter id does not match");
            Assert.That(_view.Contents, Is.EqualTo(_contents), "contents should match");
        }

        [Test]
        public void ShouldAddBookToRecent()
        {
            var builder = Container.GetService<BooksDataBuilder>();
            var recentBooks = builder.GetRecentBooks();
            Assert.That(recentBooks.Any(b => b.Id == _chapter.BookId), Is.True, "Book should be added to recent books");
        }
    }
}
