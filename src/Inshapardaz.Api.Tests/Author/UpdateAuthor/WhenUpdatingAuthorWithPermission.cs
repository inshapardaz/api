using System.Net.Http;
using System.Threading.Tasks;
using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.Helpers;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Adapters;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Author.UpdateAuthor
{
    [TestFixture(Permission.Admin)]
    [TestFixture(Permission.LibraryAdmin)]
    [TestFixture(Permission.Writer)]
    public class WhenUpdatingAuthorWithPermission : TestBase
    {
        private HttpResponseMessage _response;
        private AuthorView _expected;
        private AuthorAssert _assert;

        public WhenUpdatingAuthorWithPermission(Permission Permission)
            : base(Permission)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var authors = AuthorBuilder.WithLibrary(LibraryId).WithBooks(3).Build(4);

            var author = authors.PickRandom();

            _expected = new AuthorView { Name = Random.Name };

            _response = await Client.PutObject($"/library/{LibraryId}/authors/{author.Id}", author);
            _assert = AuthorAssert.WithResponse(_response).InLibrary(LibraryId);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
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
