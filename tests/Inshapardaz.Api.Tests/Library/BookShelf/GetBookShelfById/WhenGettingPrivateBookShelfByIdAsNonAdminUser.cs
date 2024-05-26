
using System.Net.Http;
using System.Threading.Tasks;
using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.Dto;
using Inshapardaz.Api.Tests.Helpers;
using Inshapardaz.Domain.Models;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.BookShelf.GetBookShelfById
{
    [TestFixture(Role.Reader)]
    [TestFixture(Role.Writer)]
    public class WhenGettingPrivateBookShelfByIdAsNonAdminUser : TestBase
    {
        private HttpResponseMessage _response;
        private BookShelfDto _expected;
        private BookShelfAssert _assert;

        public WhenGettingPrivateBookShelfByIdAsNonAdminUser(Role role) 
            : base(role)
        {    
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var account = AccountBuilder.Build();
            var series = BookShelfBuilder.WithLibrary(LibraryId).ForAccount(account.Id).Build(1);
            _expected = series.PickRandom();

            _response = await Client.GetAsync($"/libraries/{LibraryId}/bookshelves/{_expected.Id}");
            _assert = BookShelfAssert.WithResponse(_response).InLibrary(LibraryId);
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
