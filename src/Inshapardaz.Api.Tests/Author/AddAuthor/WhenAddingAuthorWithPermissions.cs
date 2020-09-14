using System.Net.Http;
using System.Threading.Tasks;
using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.Helpers;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Adapters;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Author.AddAuthor
{
    [TestFixture(Permission.Admin)]
    [TestFixture(Permission.LibraryAdmin)]
    [TestFixture(Permission.Writer)]
    public class WhenAddingAuthorWithPermissions : TestBase
    {
        private AuthorAssert _authorAssert;
        private HttpResponseMessage _response;

        public WhenAddingAuthorWithPermissions(Permission Permission)
            : base(Permission)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var author = new AuthorView { Name = Random.Name };

            var client = CreateClient();
            _response = await client.PostObject($"/library/{LibraryId}/authors", author);

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

            if (CurrentAuthenticationLevel == Permission.Admin ||
                CurrentAuthenticationLevel == Permission.LibraryAdmin)
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
