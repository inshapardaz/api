using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Domain.Models;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.BookShelf.DeleteBookShelf
{
    [TestFixture]
    public class WhenDeletingBookShelfOfOtherUser() : TestBase(Role.Reader)
    {
        private HttpResponseMessage _response;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var account = AccountBuilder.As(Role.Reader).Build();
            var expected = BookShelfBuilder.WithLibrary(LibraryId).ForAccount(account.Id).AsPublic().Build();

            _response = await Client.DeleteAsync($"/libraries/{LibraryId}/bookshelves/{expected.Id}");
        }

        [OneTimeTearDown]
        public void Teardown() => Cleanup();

        [Test]
        public void ShouldHaveForbiddenResult() => _response.ShouldBeForbidden();
    }
}
