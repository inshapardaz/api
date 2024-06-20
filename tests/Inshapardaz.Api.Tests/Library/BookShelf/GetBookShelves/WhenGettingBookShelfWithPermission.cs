using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.BookShelf.GetBookShelves
{
    [TestFixture(Role.Admin)]
    [TestFixture(Role.LibraryAdmin)]
    public class WhenGettingPublicBookShelfWithPermission : TestBase
    {
        private HttpResponseMessage _response;
        private PagingAssert<BookShelfView> _assert;

        public WhenGettingPublicBookShelfWithPermission(Role role)
            : base(role)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var account = AccountBuilder.Build();
            BookShelfBuilder.WithLibrary(LibraryId).WithBooks(3).AsPublic().ForAccount(account.Id).Build(4);

            _response = await Client.GetAsync($"/libraries/{LibraryId}/bookshelves?onlyPublic=true&pageNumber=1&pageSize=10");
            _assert = Services.GetService<PagingAssert<BookShelfView>>().ForResponse(_response);
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
            _assert.ShouldHaveSelfLink($"/libraries/{LibraryId}/bookshelves");
        }

        [Test]
        public void ShouldHaveCreateLink()
        {
            _assert.ShouldHaveCreateLink($"/libraries/{LibraryId}/bookshelves");
        }

        [Test]
        public void ShouldHaveEditingLinkOnBookShelf()
        {
            var expectedItems = BookShelfBuilder.BookShelves.Where(x => x.AccountId != AccountId);
            foreach (var item in expectedItems)
            {
                var actual = _assert.Data.FirstOrDefault(x => x.Id == item.Id);
                Services.GetService<BookShelfAssert>().ForView(actual).ForLibrary(LibraryId)
                      .ShouldBeSameAs(item)
                      .WithBookCount(3)
                      .WithDeleteOnlyEditableLinks()
                      .ShouldHavePublicImageLink();
            }
        }
    }
}
