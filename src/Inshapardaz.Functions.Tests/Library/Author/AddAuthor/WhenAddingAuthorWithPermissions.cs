using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Functions.Tests.Asserts;
using Inshapardaz.Functions.Tests.Helpers;
using Inshapardaz.Functions.Views.Library;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;

namespace Inshapardaz.Functions.Tests.Library.Author.AddAuthor
{
    [TestFixture(AuthenticationLevel.Administrator)]
    [TestFixture(AuthenticationLevel.Writer)]
    public class WhenAddingAuthorWithPermissions
        : LibraryTest<Functions.Library.Authors.AddAuthor>
    {
        private readonly ClaimsPrincipal _claim;
        private AuthorAssert _authorAssert;
        private CreatedResult _response;

        public WhenAddingAuthorWithPermissions(AuthenticationLevel authenticationLevel)
        {
            _claim = AuthenticationBuilder.CreateClaim(authenticationLevel);
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var _request = new AuthorView { Name = Random.Name };

            _response = (CreatedResult)await handler.Run(_request, LibraryId, _claim, CancellationToken.None);

            _authorAssert = AuthorAssert.WithResponse(_response).InLibrary(LibraryId);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldHaveCreatedResult()
        {
            _response.ShouldBeCreated();
        }

        [Test]
        public void ShouldHaveLocationHeader()
        {
            _authorAssert.ShouldHaveCorrectLocationHeader();
        }

        [Test]
        public void ShouldSaveTheAuthor()
        {
            _authorAssert.ShouldHaveSavedAuthor(DatabaseConnection);
        }

        [Test]
        public void ShouldHaveLinks()
        {
            _authorAssert.ShouldHaveSelfLink()
                         .ShouldHaveBooksLink()
                         .ShouldHaveUpdateLink()
                         .ShouldHaveDeleteLink()
                         .ShouldHaveUpdateLink();
        }
    }
}
