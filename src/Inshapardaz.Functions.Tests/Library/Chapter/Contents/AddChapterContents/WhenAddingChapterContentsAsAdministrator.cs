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

namespace Inshapardaz.Functions.Tests.Library.Chapter.Contents.AddChapterContents
{
    [TestFixture]
    public class WhenAddingChapterContentsAsAdministrator : FunctionTest
    {
        CreatedResult _response;
        ChapterContentView _view;
        string _contents;

        ChapterDataBuilder _dataBuilder;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _dataBuilder = Container.GetService<ChapterDataBuilder>();

            var chapters = _dataBuilder.WithChapters(1, true).Build();
            var chapter = chapters.First();
            _contents = new Faker().Random.Words(60);
            var handler = Container.GetService<Functions.Library.Books.Chapters.Contents.AddChapterContents>();
            var request = new RequestBuilder().WithBody(_contents).Build();
            _response = (CreatedResult) await handler.Run(request, chapter.BookId, chapter.Id, AuthenticationBuilder.AdminClaim, CancellationToken.None);

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
            var _expected = _dataBuilder.GetContentById(_view.Id);
            Assert.That(_view, Is.Not.Null, "Should return chapter");
            Assert.That(_view.Id, Is.EqualTo(_expected.Id), "Content id does not match");
            Assert.That(_view.ChapterId, Is.EqualTo(_expected.ChapterId), "Chapter id does not match");
            Assert.That(_view.Contents, Is.EqualTo(_contents), "contents should match");
        }
    }
}
