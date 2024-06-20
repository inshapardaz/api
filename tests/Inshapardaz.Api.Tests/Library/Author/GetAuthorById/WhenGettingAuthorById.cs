using System.Net.Http;
using System.Threading.Tasks;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.Author.GetAuthorById
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
        public void ShouldNotHaveImageLink()
        {
            _assert.ShouldNotHaveImageLink();
        }

        [Test]
        public void ShouldReturnCorrectAuthorData()
        {
            _assert.ShouldHaveCorrectAuthorRetunred(_expected);
        }
    }
}
