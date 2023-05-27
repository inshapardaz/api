using System.Net.Http;
using System.Threading.Tasks;
using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.Helpers;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.BookShelf.DeleteBookShelf
{
    [TestFixture]
    public class WhenDeletingBookShelfAsAnonymous : TestBase
    {
        private HttpResponseMessage _response;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var account = AccountBuilder.As(Domain.Models.Role.Reader).Build();
            var bookShelves = BookShelfBuilder.WithLibrary(LibraryId).ForAccount(account.Id).Build(4);
            var expected = bookShelves.PickRandom();

            _response = await Client.DeleteAsync($"/libraries/{LibraryId}/bookshelves/{expected.Id}");
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldHaveUnauthorizedResult()
        {
            _response.ShouldBeUnauthorized();
        }
    }
}
