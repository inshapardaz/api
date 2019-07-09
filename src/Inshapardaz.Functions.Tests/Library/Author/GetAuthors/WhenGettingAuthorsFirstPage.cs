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

namespace Inshapardaz.Functions.Tests.Library.Author.GetAuthors
{
    [TestFixture]
    public class WhenGettingAuthorsFirstPage : FunctionTest
    {
        OkObjectResult _response;
        PageView<AuthorView> _view;
        
        [OneTimeSetUp]
        public async Task Setup()
        {
            var request = new RequestBuilder()
                .WithQueryParameter("pageNumber", 1)
                .WithQueryParameter("pageSize", 10)
                .Build();

            var builder = Container.GetService<AuthorsDataBuilder>();
            builder.WithAuthors(20, 3).Build();
            
            var handler = Container.GetService<Functions.Library.Authors.GetAuthors>();
            _response = (OkObjectResult) await handler.Run(request, NullLogger.Instance, AuthenticationBuilder.Unauthorized, CancellationToken.None);

            _view = _response.Value as PageView<AuthorView>;
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
        public void ShouldHaveNextLink()
        {
            _view.Links.AssertLink("next")
                .ShouldBeGet()
                .ShouldHaveSomeHref();
        }

        [Test]
        public void ShouldNotHavePreviousLink()
        {
            _view.Links.AssertLinkNotPresent("previous");
        }

        [Test]
        public void ShouldHaveSomeAuthors()
        {
            Assert.IsNotEmpty(_view.Data, "Should return some authors.");
            Assert.That(_view.Data.Count(), Is.EqualTo(10), "Should return all authors on page");
        }

        [Test]
        public void ShouldHaveCorrectAuthorData()
        {
            var actual = _view.Data.FirstOrDefault();
            Assert.That(actual, Is.Not.Null, "Should contain at-least one author");
            Assert.That(actual.Name, Is.Not.Empty, "Author name should have a value");
            Assert.That(actual.BookCount, Is.GreaterThan(0), "Author should have some books.");

            actual.Links.AssertLinkNotPresent("update");
            actual.Links.AssertLinkNotPresent("delete");
        }
    }
}
