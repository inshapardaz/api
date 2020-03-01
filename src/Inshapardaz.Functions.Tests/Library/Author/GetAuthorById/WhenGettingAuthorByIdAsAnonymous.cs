using System.Linq;
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
    [TestFixture]
    public class WhenGettingAuthorByIdAsAnonymous : LibraryTest
    {
        private AuthorsDataBuilder _builder;
        private OkObjectResult _response;
        private AuthorView _view;
        private AuthorDto _expected;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var request = TestHelpers.CreateGetRequest();
            _builder = Container.GetService<AuthorsDataBuilder>();
            var authors = _builder.WithLibrary(LibraryId).Build(4);
            _expected = authors.First();

            var handler = Container.GetService<Functions.Library.Authors.GetAuthorById>();
            _response = (OkObjectResult)await handler.Run(request, LibraryId, _expected.Id, AuthenticationBuilder.Unauthorized, CancellationToken.None);

            _view = _response.Value as AuthorView;
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            _builder.CleanUp();
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
        public void ShouldNotHaveUpdateLink()
        {
            _view.Links.AssertLinkNotPresent("update");
        }

        [Test]
        public void ShouldNotHaveDeleteLink()
        {
            _view.Links.AssertLinkNotPresent("delete");
        }

        [Test]
        public void ShouldNotHaveImageUploadLink()
        {
            _view.Links.AssertLinkNotPresent("image-upload");
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
