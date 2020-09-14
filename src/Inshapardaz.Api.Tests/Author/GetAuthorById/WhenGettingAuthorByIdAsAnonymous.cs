using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.Dto;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Author.GetAuthorById
{
    [TestFixture]
    public class WhenGettingAuthorByIdAsAnonymous : TestBase
    {
        private HttpResponseMessage _response;
        private AuthorDto _expected;
        private AuthorAssert _assert;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var authors = AuthorBuilder.WithLibrary(LibraryId).Build(4);
            _expected = authors.First();

            var client = CreateClient();
            _response = await client.GetAsync($"/library/{LibraryId}/authors/{_expected.Id}");
            _assert = AuthorAssert.WithResponse(_response).InLibrary(LibraryId);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldReturnOk()
        {
            _response.ShouldBeOk();
        }

        [Test]
        public void ShouldHaveSelfLink()
        {
            _assert.ShouldHaveSelfLink();
        }

        [Test]
        public void ShouldHaveBooksLink()
        {
            _assert.ShouldHaveBooksLink();
        }

        [Test]
        public void ShouldNotHaveUpdateLink()
        {
            _assert.ShouldNotHaveUpdateLink();
        }

        [Test]
        public void ShouldNotHaveDeleteLink()
        {
            _assert.ShouldNotHaveDeleteLink();
        }

        [Test]
        public void ShouldNotHaveImageUploadLink()
        {
            _assert.ShouldNotHaveImageUploadLink();
        }

        [Test]
        public void ShouldReturnCorrectAuthorData()
        {
            _assert.ShouldHaveCorrectAuthorRetunred(_expected, DatabaseConnection);
        }
    }
}
