using System.Linq;
using System.Security.Claims;
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

namespace Inshapardaz.Functions.Tests.Library.Book.GetLatestBooks
{
    [TestFixture]
    public class WhenGettingLatestBooksAsWriter : FunctionTest
    {
        private OkObjectResult _response;
        private ListView<BookView> _view;
        
        [OneTimeSetUp]
        public async Task Setup()
        {
            var request = TestHelpers.CreateGetRequest();

            var builder = Container.GetService<BooksDataBuilder>();
            builder.Build(40);

            var handler = Container.GetService<Functions.Library.Books.GetLatestBooks>();
            _response = (OkObjectResult) await handler.Run(request, NullLogger.Instance, AuthenticationBuilder.WriterClaim, CancellationToken.None);

            _view = _response.Value as ListView<BookView>;
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldHaveOkResult()
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
        public void ShouldReturn10Books()
        {
            Assert.That(_view.Items.Count(), Is.EqualTo(10));
        }

        [Test]
        public void ShouldHaveCorrectBookData()
        {
            var actual = _view.Items.FirstOrDefault();
            Assert.That(actual, Is.Not.Null, "Should contain at-least one book");
            Assert.That(actual.Title, Is.Not.Empty, "Book name should have a value");
            Assert.That(actual.Description, Is.Not.Empty, "Book should have some description.");

            actual.Links.AssertLink("update")
                  .ShouldBePut()
                  .ShouldHaveSomeHref();
            actual.Links.AssertLink("delete")
                  .ShouldBeDelete()
                  .ShouldHaveSomeHref();

        }
    }
}
