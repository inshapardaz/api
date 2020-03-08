using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Bogus;
using Inshapardaz.Functions.Tests.DataBuilders;
using Inshapardaz.Functions.Tests.DataHelpers;
using Inshapardaz.Functions.Tests.Helpers;
using Inshapardaz.Functions.Views.Library;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Functions.Tests.Library.Author.UpdateAuthor
{
    [TestFixture(AuthenticationLevel.Administrator)]
    [TestFixture(AuthenticationLevel.Writer)]
    public class WhenUpdatingAuthorWithPermission : LibraryTest<Functions.Library.Authors.UpdateAuthor>
    {
        private OkObjectResult _response;
        private AuthorsDataBuilder _dataBuilder;
        private AuthorView _expected;
        private readonly ClaimsPrincipal _claim;

        public WhenUpdatingAuthorWithPermission(AuthenticationLevel authenticationLevel)
        {
            _claim = AuthenticationBuilder.CreateClaim(authenticationLevel);
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            _dataBuilder = Container.GetService<AuthorsDataBuilder>();

            var authors = _dataBuilder.WithLibrary(LibraryId).WithBooks(3).Build(4);

            var author = authors.First();

            _expected = new AuthorView { Name = new Faker().Name.FullName() };

            _response = (OkObjectResult)await handler.Run(_expected, LibraryId, author.Id, _claim, CancellationToken.None);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            _dataBuilder.CleanUp();
            Cleanup();
        }

        [Test]
        public void ShouldHaveCreatedResult()
        {
            Assert.That(_response, Is.Not.Null);
            Assert.That(_response.StatusCode, Is.EqualTo((int)HttpStatusCode.OK));
        }

        [Test]
        public void ShouldHaveUpdatedTheAuthor()
        {
            var returned = _response.Value as AuthorView;
            Assert.That(returned, Is.Not.Null);

            var actual = DatabaseConnection.GetAuthorById(returned.Id);
            Assert.That(actual.Name, Is.EqualTo(_expected.Name), "Author should have expected name.");
        }
    }
}
