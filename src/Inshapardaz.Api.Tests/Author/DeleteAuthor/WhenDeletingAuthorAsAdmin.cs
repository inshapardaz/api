using System.Net.Http;
using System.Threading.Tasks;
using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.Dto;
using Inshapardaz.Api.Tests.Helpers;
using Inshapardaz.Domain.Adapters;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Author.DeleteAuthor
{
    [TestFixture(Permission.Admin)]
    [TestFixture(Permission.LibraryAdmin)]
    public class WhenDeletingAuthorAsAdmin : TestBase
    {
        private HttpResponseMessage _response;

        private AuthorDto _expected;

        public WhenDeletingAuthorAsAdmin(Permission Permission)
            : base(Permission)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var authors = AuthorBuilder.WithLibrary(LibraryId).WithBooks(0).Build(4);
            _expected = authors.PickRandom();

            var client = CreateClient();
            _response = await client.DeleteAsync($"/library/{LibraryId}/authors/{_expected.Id}");
        }

        [OneTimeTearDown]
        public void Teardown()
        {
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
