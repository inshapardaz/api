using System.Net.Http;
using System.Threading.Tasks;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Domain.Models;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.BookShelf.UpdateBookShelf
{
    [TestFixture]
    public class WhenUpdatingBookShelfBlongingToOtherUser : TestBase
    {
        private HttpResponseMessage _response;

        public WhenUpdatingBookShelfBlongingToOtherUser() 
            : base(Role.Reader)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var account = AccountBuilder.As(Role.Reader).Build();
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
            _response.ShouldBeForbidden();
        }
    }
}
