using System.Net.Http;
using System.Threading.Tasks;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.Author.GetAuthorById
{
    [TestFixture]
    public class WhenGettingAuthorByIdAsReader : TestBase
    {
        private HttpResponseMessage _response;
        private AuthorDto _expected;
        private AuthorAssert _assert;

        public WhenGettingAuthorByIdAsReader()
            : base(Role.Reader)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var authors = AuthorBuilder.WithLibrary(LibraryId).Build(4);
            _expected = authors.PickRandom();

            _response = await Client.GetAsync($"/libraries/{LibraryId}/authors/{_expected.Id}");
            _assert = Services.GetService<AuthorAssert>().ForResponse(_response).ForLibrary(LibraryId);
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
            _assert.ShouldHaveCorrectAuthorRetunred(_expected);
        }
    }
}
