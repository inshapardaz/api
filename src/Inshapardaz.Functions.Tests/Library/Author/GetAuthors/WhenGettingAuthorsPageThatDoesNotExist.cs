using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Functions.Tests.DataBuilders;
using Inshapardaz.Functions.Tests.Helpers;
using Inshapardaz.Functions.Views;
using Inshapardaz.Functions.Views.Library;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Functions.Tests.Library.Author.GetAuthors
{
    [TestFixture]
    public class WhenGettingAuthorsPageThatDoesNotExist : FunctionTest
    {
        private AuthorsDataBuilder _builder;
        private OkObjectResult _response;
        private PageView<AuthorView> _view;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var request = new RequestBuilder()
                .WithQueryParameter("pageNumber", 100)
                .WithQueryParameter("pageSize", 10)
                .Build();

            _builder = Container.GetService<AuthorsDataBuilder>();
            _builder.WithBooks(3).Build(20);

            var handler = Container.GetService<Functions.Library.Authors.GetAuthors>();
            _response = (OkObjectResult)await handler.Run(request, _builder.Library.Id, AuthenticationBuilder.Unauthorized, CancellationToken.None);

            _view = _response.Value as PageView<AuthorView>;
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            _builder.CleanUp();
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
        public void ShouldNotHavePreviousLink()
        {
            _view.Links.AssertLinkNotPresent("previous");
        }

        [Test]
        public void ShouldNotReturnAuthor()
        {
            Assert.IsEmpty(_view.Data, "Should return no data.");
        }
    }
}
