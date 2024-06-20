using System.Net.Http;
using System.Threading.Tasks;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.BookShelf.GetBookShelfById
{
    [TestFixture(Role.Admin)]
    [TestFixture(Role.LibraryAdmin)]
    [TestFixture(Role.Writer)]
    [TestFixture(Role.Reader)]
    public class WhenGettingBookShelfWithPermission : TestBase
    {
        private HttpResponseMessage _response;
        private BookShelfDto _expected;
        private BookShelfAssert _assert;

        public WhenGettingBookShelfWithPermission(Role role)
            : base(role)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var bookShelf = BookShelfBuilder.WithLibrary(LibraryId).ForAccount(AccountId).Build(4);
            _expected = bookShelf.PickRandom();

            _response = await Client.GetAsync($"/libraries/{LibraryId}/bookshelves/{_expected.Id}");
            _assert = Services.GetService<BookShelfAssert>().ForResponse(_response).ForLibrary(LibraryId);
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
        public void ShouldHaveDeleteLink()
        {
            _assert.ShouldHaveDeleteLink();
        }

        [Test]
        public void ShouldHaveImageUploadLink()
        {
            _assert.ShouldHaveImageUploadLink();
        }

        [Test]
        public void ShouldReturnCorrectBookShelfData()
        {
            _assert.ShouldHaveCorrectBookShelfRetunred(_expected);
        }
    }
}
