using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Functions.Tests.DataBuilders;
using Inshapardaz.Functions.Tests.DataHelpers;
using Inshapardaz.Functions.Tests.Dto;
using Inshapardaz.Functions.Tests.Helpers;
using Inshapardaz.Functions.Views.Library;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Functions.Tests.Library.Author.GetAuthorById
{
    [TestFixture(AuthenticationLevel.Administrator)]
    [TestFixture(AuthenticationLevel.Writer)]
    public class WhenGettingAuthorWithPermission : FunctionTest
    {
        private AuthorsDataBuilder _builder;
        private OkObjectResult _response;
        private AuthorView _view;
        private AuthorDto _expected;
        private readonly ClaimsPrincipal _claim;

        public WhenGettingAuthorWithPermission(AuthenticationLevel authenticationLevel)
        {
            _claim = AuthenticationBuilder.CreateClaim(authenticationLevel);
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var request = TestHelpers.CreateGetRequest();
            _builder = Container.GetService<AuthorsDataBuilder>();
            var authors = _builder.Build(4);
            _expected = authors.First();

            var handler = Container.GetService<Functions.Library.Authors.GetAuthorById>();
            _response = (OkObjectResult)await handler.Run(request, _builder.Library.Id, _expected.Id, _claim, CancellationToken.None);

            _view = _response.Value as AuthorView;
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
        public void ShouldHaveBooksLink()
        {
            _view.Links.AssertLink("books")
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
        public void ShouldReturnCorrectAuthorData()
        {
            Assert.That(_view, Is.Not.Null, "Should contain at-least one author");
            Assert.That(_view.Id, Is.EqualTo(_expected.Id), "Author id does not match");
            Assert.That(_view.Name, Is.EqualTo(_expected.Name), "Author name does not match");

            var authorBookCount = DatabaseConnection.GetBookCountByAuthor(_expected.Id);
            Assert.That(_view.BookCount, Is.EqualTo(authorBookCount), "Author book count does not match");
        }
    }
}
