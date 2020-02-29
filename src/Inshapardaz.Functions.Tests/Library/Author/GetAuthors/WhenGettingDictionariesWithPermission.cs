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
using NUnit.Framework;

namespace Inshapardaz.Functions.Tests.Library.Author.GetAuthors
{
    [TestFixture(AuthenticationLevel.Administrator)]
    [TestFixture(AuthenticationLevel.Writer)]
    public class WhenGettingDictionariesWithPermission : FunctionTest
    {
        private AuthorsDataBuilder _builder;
        private OkObjectResult _response;
        private PageView<AuthorView> _view;
        private readonly ClaimsPrincipal _claim;

        public WhenGettingDictionariesWithPermission(AuthenticationLevel authenticationLevel)
        {
            _claim = AuthenticationBuilder.CreateClaim(authenticationLevel);
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var request = TestHelpers.CreateGetRequest();

            _builder = Container.GetService<AuthorsDataBuilder>();
            _builder.WithBooks(3).Build(4);

            var handler = Container.GetService<Functions.Library.Authors.GetAuthors>();
            _response = (OkObjectResult)await handler.Run(request, _builder.Library.Id, _claim, CancellationToken.None);

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
        public void ShouldHaveCreateLink()
        {
            _view.Links.AssertLink("create")
                .ShouldBePost()
                .ShouldHaveSomeHref();
        }

        [Test]
        public void ShouldHaveEditingLinkOnAuthor()
        {
            var actual = _view.Data.FirstOrDefault();
            actual.Links.AssertLink("update")
                .ShouldBePut()
                .ShouldHaveSomeHref();
            actual.Links.AssertLink("delete")
                .ShouldBeDelete()
                .ShouldHaveSomeHref();
        }
    }
}
