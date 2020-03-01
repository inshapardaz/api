using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Functions.Tests.DataBuilders;
using Inshapardaz.Functions.Tests.Helpers;
using Inshapardaz.Functions.Views;
using Inshapardaz.Functions.Views.Library;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Functions.Tests.Library.Book.GetBooksByCategory
{
    [TestFixture]
    public class WhenGettingBooksByCategoryPageThatDoesNotExist : LibraryTest
    {
        private OkObjectResult _response;
        private PageView<BookView> _view;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var request = new RequestBuilder()
                          .WithQueryParameter("pageNumber", 3)
                          .WithQueryParameter("pageSize", 10)
                          .Build();

            var builder = Container.GetService<BooksDataBuilder>();
            builder.WithCategories(2).Build(20);

            var categoriesBuilder = Container.GetService<CategoriesDataBuilder>();
            var category = categoriesBuilder.Build();
            // builder.WithCategory(category).Build(20);

            var handler = Container.GetService<Functions.Library.Books.GetBooksByCategory>();
            _response = (OkObjectResult)await handler.Run(request, LibraryId, category.Id, AuthenticationBuilder.ReaderClaim, CancellationToken.None);

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
        public void ShouldNotHaveNextLink()
        {
            _view.Links.AssertLinkNotPresent("next");
        }

        [Test]
        public void ShouldHavePreviousLink()
        {
            _view.Links.AssertLinkNotPresent("previous");
        }

        [Test]
        public void ShouldNotHaveAnyBooks()
        {
            Assert.IsEmpty(_view.Data, "Should return no books.");
        }

        [Test]
        public void ShouldReturnCorrectPage()
        {
            Assert.That(_view.PageCount, Is.EqualTo(2));
            Assert.That(_view.PageSize, Is.EqualTo(10));
            Assert.That(_view.TotalCount, Is.EqualTo(20));
            Assert.That(_view.CurrentPageIndex, Is.EqualTo(3));
        }
    }
}
