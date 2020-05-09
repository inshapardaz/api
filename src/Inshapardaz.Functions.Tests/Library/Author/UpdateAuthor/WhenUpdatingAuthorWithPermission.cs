using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Bogus;
using FluentAssertions;
using Inshapardaz.Functions.Tests.Asserts;
using Inshapardaz.Functions.Tests.DataBuilders;
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
        private AuthorAssert _assert;

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
            _assert = AuthorAssert.WithResponse(_response).InLibrary(LibraryId);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            _dataBuilder.CleanUp();
            Cleanup();
        }

        [Test]
        public void ShouldHaveOkResult()
        {
            _response.ShouldBeOk();
        }

        [Test]
        public void ShouldHaveUpdatedTheAuthor()
        {
            _assert.ShouldHaveSavedAuthor(DatabaseConnection);
        }
    }
}
