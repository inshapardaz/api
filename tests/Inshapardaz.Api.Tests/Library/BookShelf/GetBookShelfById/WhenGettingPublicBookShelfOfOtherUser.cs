using System.Net.Http;
using System.Threading.Tasks;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Domain.Models;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.BookShelf.GetBookShelfById
{
    [TestFixture(Role.Admin)]
    [TestFixture(Role.LibraryAdmin)]
    [TestFixture(Role.Writer)]
    [TestFixture(Role.Reader)]
    public class WhenGettingPublicBookShelfOfOtherUser : TestBase
    {
        private HttpResponseMessage _response;
        private BookShelfDto _expected;
        private BookShelfAssert _assert;

        public WhenGettingPublicBookShelfOfOtherUser(Role role)
            : base(role)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var account = AccountBuilder.Build();
            var bookShelf = BookShelfBuilder.WithLibrary(LibraryId).AsPublic().ForAccount(account.Id).Build(4);
            _expected = bookShelf.PickRandom();

            _response = await Client.GetAsync($"/libraries/{LibraryId}/bookshelves/{_expected.Id}");
            _assert = BookShelfAssert.WithResponse(_response).InLibrary(LibraryId);
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
        public void ShouldHaveCorrectDeleteLink()
        {
            if (_role == Role.Admin || _role == Role.LibraryAdmin)
            {
                _assert.ShouldHaveDeleteLink();
            }
            else
            {
                _assert.ShouldNotHaveDeleteLink();
            }
        }

        [Test]
        public void ShouldNotHaveImageUploadLink()
        {
            _assert.ShouldNotHaveImageUploadLink();
        }

        [Test]
        public void ShouldReturnCorrectSeriesData()
        {
            _assert.ShouldHaveCorrectBookShelfRetunred(_expected, DatabaseConnection);
        }
    }
}
