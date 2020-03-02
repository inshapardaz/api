using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Functions.Tests.DataBuilders;
using Inshapardaz.Functions.Tests.DataHelpers;
using Inshapardaz.Functions.Tests.Dto;
using Inshapardaz.Functions.Tests.Helpers;
using Inshapardaz.Ports.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Functions.Tests.Library.Author.DeleteAuthor
{
    [TestFixture(AuthenticationLevel.Administrator)]
    [TestFixture(AuthenticationLevel.Writer)]
    public class WhenDeletingAuthorWithPermission : LibraryTest<Functions.Library.Authors.DeleteAuthor>
    {
        private NoContentResult _response;

        private AuthorDto _expected;
        private AuthorsDataBuilder _dataBuilder;
        private readonly ClaimsPrincipal _claim;

        public WhenDeletingAuthorWithPermission(AuthenticationLevel authenticationLevel)
        {
            _claim = AuthenticationBuilder.CreateClaim(authenticationLevel);
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var request = TestHelpers.CreateGetRequest();
            _dataBuilder = Container.GetService<AuthorsDataBuilder>();
            var authors = _dataBuilder.WithLibrary(LibraryId).Build(4);
            _expected = authors.First();

            _response = (NoContentResult)await handler.Run(request, LibraryId, _expected.Id, _claim, CancellationToken.None);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            _dataBuilder.CleanUp();
            Cleanup();
        }

        [Test]
        public void ShouldReturnOk()
        {
            Assert.That(_response, Is.Not.Null);
            Assert.That(_response.StatusCode, Is.EqualTo(204));
        }

        [Test]
        public void ShouldHaveDeletedAuthor()
        {
            var cat = _dataBuilder.GetById(_expected.Id);
            Assert.That(cat, Is.Null, "Author should be deleted.");
        }

        [Test]
        public void ShouldHaveDeletedTheAuthorImage()
        {
            var db = Container.GetService<IDatabaseContext>();
            var file = db.File.Where(i => i.Id == _expected.ImageId);
            Assert.That(file, Is.Empty, "Author Image should be deleted");
        }

        [Test]
        public void ShouldHaveDeletedTheAuthorBooks()
        {
            var authorBooks = DatabaseConnection.GetBooksByAuthor(_expected.Id);
            Assert.That(authorBooks, Is.Empty, "Author Books should be deleted");
        }
    }
}
