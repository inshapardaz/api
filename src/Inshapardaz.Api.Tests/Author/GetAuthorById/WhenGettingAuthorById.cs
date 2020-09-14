using System.Net.Http;
using System.Threading.Tasks;
using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.Dto;
using Inshapardaz.Api.Tests.Helpers;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Author.GetAuthorById
{
    [TestFixture]
    public class WhenGettingAuthorById : TestBase
    {
        private HttpResponseMessage _response;
        private AuthorDto _expected;
        private AuthorAssert _assert;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var authors = AuthorBuilder.WithLibrary(LibraryId).WithoutImage().WithBooks(3).Build(4);
            _expected = authors.PickRandom();

            _response = await Client.GetAsync($"/library/{LibraryId}/authors/{_expected.Id}");
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
        public void ShouldNotHaveImageLink()
        {
            _assert.ShouldNotHaveImageLink();
        }

        [Test]
        public void ShouldReturnCorrectAuthorData()
        {
            _assert.ShouldHaveCorrectAuthorRetunred(_expected, DatabaseConnection);
        }
    }
}
