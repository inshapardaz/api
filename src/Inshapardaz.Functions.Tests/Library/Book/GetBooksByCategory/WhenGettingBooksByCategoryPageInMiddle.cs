using System.Linq;
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
    public class WhenGettingBooksByCategoryPageInMiddle : FunctionTest
    {
        private OkObjectResult _response;
        private PageView<BookView> _view;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var request = new RequestBuilder()
                          .WithQueryParameter("pageNumber", 2)
                          .WithQueryParameter("pageSize", 10)
                          .Build();

            var builder = Container.GetService<BooksDataBuilder>();
            builder.WithCategories(2).Build(20);

            var categoriesBuilder = Container.GetService<CategoriesDataBuilder>();
            var category = categoriesBuilder.Build();
            //builder.WithCategory(category).Build(30);

            var handler = Container.GetService<Functions.Library.Books.GetBooksByCategory>();
            _response = (OkObjectResult)await handler.Run(request, category.Id, AuthenticationBuilder.ReaderClaim, CancellationToken.None);

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
        public void ShouldHaveNextLink()
        {
            _view.Links.AssertLink("next")
                 .ShouldBeGet()
                 .ShouldHaveSomeHref();
        }

        [Test]
        public void ShouldNotHavePreviousLink()
        {
            _view.Links.AssertLink("previous")
                 .ShouldBeGet()
                 .ShouldHaveSomeHref();
        }

        [Test]
        public void ShouldHaveSomeBooks()
        {
            Assert.IsNotEmpty(_view.Data, "Should return some books.");
            Assert.That(_view.Data.Count(), Is.EqualTo(10), "Should return all books on page");
        }

        [Test]
        public void ShouldReturnCorrectPage()
        {
            Assert.That(_view.PageCount, Is.EqualTo(3));
            Assert.That(_view.PageSize, Is.EqualTo(10));
            Assert.That(_view.TotalCount, Is.EqualTo(30));
            Assert.That(_view.CurrentPageIndex, Is.EqualTo(2));
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
        }
    }
}
