using System.Net.Http;
using System.Threading.Tasks;
using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.Helpers;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Models;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Author.AddAuthor
{
    [TestFixture(Role.Admin)]
    [TestFixture(Role.LibraryAdmin)]
    [TestFixture(Role.Writer)]
    public class WhenAddingAuthorWithPermissions : TestBase
    {
        private AuthorAssert _authorAssert;
        private HttpResponseMessage _response;

        public WhenAddingAuthorWithPermissions(Role role)
            : base(role)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var author = new AuthorView { Name = Random.Name };

            _response = await Client.PostObject($"/libraries/{LibraryId}/authors", author);

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
                         .ShouldHaveImageUpdateLink();

            if (CurrentAuthenticationLevel == Role.Admin ||
                CurrentAuthenticationLevel == Role.LibraryAdmin)
            {
                _authorAssert.ShouldHaveDeleteLink();
            }
            else
            {
                _authorAssert.ShouldNotHaveDeleteLink();
            }
        }
    }
}