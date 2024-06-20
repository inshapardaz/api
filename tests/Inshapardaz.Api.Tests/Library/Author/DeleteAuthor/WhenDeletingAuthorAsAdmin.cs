using System.Net.Http;
using System.Threading.Tasks;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.Author.DeleteAuthor
{
    [TestFixture(Role.Admin)]
    [TestFixture(Role.LibraryAdmin)]
    public class WhenDeletingAuthorAsAdmin : TestBase
    {
        private HttpResponseMessage _response;
        private AuthorAssert _authorAssert;
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

            _filePath = FileTestRepository.GetFileById(_expected.ImageId.Value)?.FilePath;
            _response = await Client.DeleteAsync($"/libraries/{LibraryId}/authors/{_expected.Id}");
            _authorAssert = Services.GetService<AuthorAssert>().ForResponse(_response).ForLibrary(LibraryId);
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
            _authorAssert.ShouldHaveDeletedAuthor(_expected.Id);
        }

        [Test]
        public void ShouldHaveDeletedTheAuthorImage()
        {
            _authorAssert.ShouldHaveDeletedAuthorImage(_expected.Id, _expected.ImageId.Value, _filePath);
        }

        [Test]
        public void ShouldDeleteAuthorBooks()
        {
            Assert.Inconclusive("Define a policy and implement");
        }
    }
}
