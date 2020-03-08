using System.Threading;
using System.Threading.Tasks;
using Bogus;
using Inshapardaz.Functions.Tests.DataBuilders;
using Inshapardaz.Functions.Tests.DataHelpers;
using Inshapardaz.Functions.Tests.Helpers;
using Inshapardaz.Functions.Views.Library;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Functions.Tests.Library.Chapter.Contents.AddChapterContents
{
    [TestFixture]
    public class WhenAddingChapterContentsAsAdministrator
        : LibraryTest<Functions.Library.Books.Chapters.Contents.AddChapterContents>
    {
        private CreatedResult _response;
        private ChapterContentView _view;
        private string _contents;

        private ChapterDataBuilder _dataBuilder;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _dataBuilder = Container.GetService<ChapterDataBuilder>();

            var chapter = _dataBuilder.AsPublic().Build();
            _contents = new Faker().Random.Words(60);
            var request = new RequestBuilder().WithBody(_contents).Build();
            _response = (CreatedResult)await handler.Run(request, LibraryId, chapter.BookId, chapter.Id, AuthenticationBuilder.AdminClaim, CancellationToken.None);

            _view = _response.Value as ChapterContentView;
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
            Assert.That(_response.StatusCode, Is.EqualTo(201));
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
            var expected = DatabaseConnection.GetContentById(_view.Id);
            Assert.That(_view, Is.Not.Null, "Should return chapter");
            Assert.That(_view.Id, Is.EqualTo(expected.Id), "Content id does not match");
            Assert.That(_view.ChapterId, Is.EqualTo(expected.ChapterId), "Chapter id does not match");
            Assert.That(_view.Contents, Is.EqualTo(_contents), "contents should match");
        }
    }
}
