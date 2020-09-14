using System.Net.Http;
using System.Threading.Tasks;
using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.Dto;
using Inshapardaz.Api.Tests.Helpers;
using Inshapardaz.Domain.Adapters;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Author.GetAuthorById
{
    [TestFixture(Permission.Admin)]
    [TestFixture(Permission.LibraryAdmin)]
    [TestFixture(Permission.Writer)]
    public class WhenGettingAuthorWithPermission : TestBase
    {
        private HttpResponseMessage _response;
        private AuthorDto _expected;
        private AuthorAssert _assert;

        public WhenGettingAuthorWithPermission(Permission Permission)
            : base(Permission)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var authors = AuthorBuilder.WithLibrary(LibraryId).Build(4);
            _expected = authors.PickRandom();

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
        public void ShouldHaveUpdateLink()
        {
            _assert.ShouldHaveUpdateLink();
        }

        [Test]
        public void ShouldNotHaveDeleteLink()
        {
            if (CurrentAuthenticationLevel == Permission.LibraryAdmin ||
                CurrentAuthenticationLevel == Permission.Admin)
            {
                _assert.ShouldHaveDeleteLink();
            }
            else
            {
                _assert.ShouldNotHaveDeleteLink();
            }
        }

        [Test]
        public void ShouldHaveImageUploadLink()
        {
            _assert.ShouldHaveImageUploadLink();
        }

        [Test]
        public void ShouldReturnCorrectAuthorData()
        {
            _assert.ShouldHaveCorrectAuthorRetunred(_expected, DatabaseConnection);
        }
    }
}
