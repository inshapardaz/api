using System.Net.Http;
using System.Threading.Tasks;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.Author.AddAuthor
{
    [TestFixture(Role.Admin)]
    [TestFixture(Role.LibraryAdmin)]
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
            var author = new AuthorView { Name = RandomData.Name };

            _response = await Client.PostObject($"/libraries/{LibraryId}/authors", author);

            _authorAssert = Services.GetService<AuthorAssert>().ForResponse(_response).ForLibrary(LibraryId);
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
            _authorAssert.ShouldHaveSavedAuthor();
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
