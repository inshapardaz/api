using System.Net.Http;
using System.Threading.Tasks;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Helpers;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.BookShelf.GetBookShelfById
{
    [TestFixture]
    public class WhenGettingPrivateBookShelfByIdAsAnonymous : TestBase
    {
        private HttpResponseMessage _response;
        private BookShelfDto _expected;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var account = AccountBuilder.Build();
            var series = BookShelfBuilder.WithLibrary(LibraryId).ForAccount(account.Id).Build(1);
            _expected = series.PickRandom();

            _response = await Client.GetAsync($"/libraries/{LibraryId}/bookshelves/{_expected.Id}");
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldReturnNotFound()
        {
            _response.ShouldBeNotFound();
        }
    }
}
