using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Functions.Tests.DataBuilders;
using Inshapardaz.Functions.Tests.Helpers;
using Inshapardaz.Functions.Views.Library;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Functions.Tests.Library.Book.GetBookById
{
    [TestFixture]
    public class WhenGettingBookByIdAsWriter : LibraryTest<Functions.Library.Books.GetBookById>
    {
        private OkObjectResult _response;
        private BookView _view;
        private Ports.Database.Entities.Library.Book _expected;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var request = TestHelpers.CreateGetRequest();
            var builder = Container.GetService<BooksDataBuilder>();
            var books = builder.Build(4);
            _expected = books.First();

            _response = (OkObjectResult)await handler.Run(request, LibraryId, _expected.Id, AuthenticationBuilder.WriterClaim, CancellationToken.None);

            _view = _response.Value as BookView;
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
        public void ShouldHaveAuthorLink()
        {
            _view.Links.AssertLink("author")
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
        public void ShouldHaveImageUploadLink()
        {
            _view.Links.AssertLink("image-upload")
                 .ShouldBePut()
                 .ShouldHaveSomeHref();
        }

        [Test]
        public void ShouldHaveCreateChapterLink()
        {
            _view.Links.AssertLink("create-chapter")
                 .ShouldBePost()
                 .ShouldHaveSomeHref();
        }

        [Test]
        public void ShouldHaveAddFileLink()
        {
            _view.Links.AssertLink("add-file")
                 .ShouldBePost()
                 .ShouldHaveSomeHref();
        }

        [Test]
        public void ShouldReturnCorrectBookData()
        {
            Assert.That(_view, Is.Not.Null, "Should return a book");
            Assert.That(_view.Id, Is.EqualTo(_expected.Id), "Book id does not match");
            Assert.That(_view.Title, Is.EqualTo(_expected.Title), "Book title does not match");
            Assert.That(_view.Description, Is.EqualTo(_expected.Description), "Book description count does not match");
        }
    }
}
