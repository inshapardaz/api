using System.Net.Http;
using System.Threading.Tasks;
using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.Helpers;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.BookShelf.UpdateBookShelf
{
    [TestFixture]
    public class WhenUpdatingBookShelfAsUnauthorized : TestBase
    {
        private HttpResponseMessage _response;


        [OneTimeSetUp]
        public async Task Setup()
        {
            var account = AccountBuilder.As(Domain.Models.Role.Reader).Build();
            var bookShelf = BookShelfBuilder.WithLibrary(LibraryId).ForAccount(account.Id).Build();
            bookShelf.Name = RandomData.Name;

            _response = await Client.PutObject($"/libraries/{LibraryId}/bookshelves/{bookShelf.Id}", bookShelf);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldHaveForbiddenResult()
        {
            _response.ShouldBeUnauthorized();
        }
    }
}
