using System.Net.Http;
using System.Threading.Tasks;
using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.DataHelpers;
using Inshapardaz.Api.Tests.Dto;
using Inshapardaz.Api.Tests.Helpers;
using Inshapardaz.Domain.Models;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.Author.DeleteAuthor
{
    [TestFixture(Role.Admin)]
    [TestFixture(Role.LibraryAdmin)]
    public class WhenDeletingAuthorAsAdmin : TestBase
    {
        private HttpResponseMessage _response;

        private AuthorDto _expected;
        private string _filePath;

        public WhenDeletingAuthorAsAdmin(Role role)
            : base(role)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var authors = AuthorBuilder.WithLibrary(LibraryId).WithBooks(0).Build(4);
            _expected = authors.PickRandom();

            _filePath = DatabaseConnection.GetFileById(_expected.ImageId.Value)?.FilePath;
            _response = await Client.DeleteAsync($"/libraries/{LibraryId}/authors/{_expected.Id}");
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
            AuthorAssert.ShouldHaveDeletedAuthorImage(_expected.Id, _expected.ImageId.Value, _filePath, DatabaseConnection, FileStore);
        }

        [Test]
        public void ShouldDeleteAuthorBooks()
        {
            Assert.Inconclusive("Define a policy and implement");
        }
    }
}
