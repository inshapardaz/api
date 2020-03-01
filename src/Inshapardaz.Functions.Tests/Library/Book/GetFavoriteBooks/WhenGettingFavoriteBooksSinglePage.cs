using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Functions.Tests.DataBuilders;
using Inshapardaz.Functions.Tests.Helpers;
using Inshapardaz.Functions.Views;
using Inshapardaz.Functions.Views.Library;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;

namespace Inshapardaz.Functions.Tests.Library.Book.GetFavoriteBooks
{
    [TestFixture]
    public class WhenGettingFavoriteBooksSinglePage : LibraryTest
    {
        private OkObjectResult _response;
        private PageView<BookView> _view;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var request = new RequestBuilder()
                          .WithQueryParameter("pageNumber", 1)
                          .WithQueryParameter("pageSize", 20)
                          .Build();

            var claim = AuthenticationBuilder.ReaderClaim;

            var builder = Container.GetService<BooksDataBuilder>();
            var books = builder.Build(40);
            builder.AddSomeToFavorite(books, claim.GetUserId(), 12);

            var handler = Container.GetService<Functions.Library.Books.GetFavoriteBooks>();
            _response = (OkObjectResult)await handler.Run(request, LibraryId, claim, CancellationToken.None);

            _view = _response.Value as PageView<BookView>;
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
        public void ShouldNotHaveCreateLink()
        {
            _view.Links.AssertLinkNotPresent("create");
        }

        [Test]
        public void ShouldReturnCorrectPage()
        {
            Assert.That(_view.PageCount, Is.EqualTo(1));
            Assert.That(_view.PageSize, Is.EqualTo(20));
            Assert.That(_view.TotalCount, Is.EqualTo(12));
            Assert.That(_view.CurrentPageIndex, Is.EqualTo(1));
        }

        [Test]
        public void ShouldNotHaveNextLink()
        {
            _view.Links.AssertLinkNotPresent("next");
        }

        [Test]
        public void ShouldNotHavePreviousLink()
        {
            _view.Links.AssertLinkNotPresent("previous");
        }

        [Test]
        public void ShouldHaveCorrectBookData()
        {
            var actual = _view.Data.FirstOrDefault();
            Assert.That(actual, Is.Not.Null, "Should contain at-least one book");
            Assert.That(actual.Title, Is.Not.Empty, "Book name should have a value");
            Assert.That(actual.Description, Is.Not.Empty, "Book should have some description.");

            actual.Links.AssertLinkNotPresent("update");
            actual.Links.AssertLinkNotPresent("delete");

            actual.Links.AssertLink("remove-favorite")
                  .ShouldBeDelete()
                  .ShouldHaveSomeHref();
        }
    }
}
