using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Functions.Tests.Asserts;
using Inshapardaz.Functions.Tests.DataBuilders;
using Inshapardaz.Functions.Tests.Dto;
using Inshapardaz.Functions.Tests.Helpers;
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
            var authors = _dataBuilder.WithLibrary(LibraryId).WithBooks(0).Build(4);
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
        public void ShouldReturnNoContent()
        {
            _response.ShouldBeNoContent();
        }

        [Test]
        public void ShouldHaveDeletedAuthor()
        {
            AuthorAssert.ShouldHaveDeletedAuthor(_expected.Id, DatabaseConnection);
        }

        [Test]
        public void ShouldHaveDeletedTheAuthorImage()
        {
            AuthorAssert.ShouldHaveDeletedAuthorImage(_expected.Id, DatabaseConnection);
        }

        [Test]
        public void ShouldDeleteAuthorBooks()
        {
            Assert.Inconclusive("Define a policy and implement");
        }
    }
}
