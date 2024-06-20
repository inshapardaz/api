using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.BookShelf.GetBookShelves
{
    [TestFixture]
    public class WhenGettingPublicBookShelves : TestBase
    {
        private HttpResponseMessage _response;
        private PagingAssert<BookShelfView> _assert;

        public WhenGettingPublicBookShelves()
            : base(Role.Reader)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var account = AccountBuilder.Build();
            BookShelfBuilder.WithLibrary(LibraryId).WithBooks(3).AsPublic().ForAccount(account.Id).Build(4);
            BookShelfBuilder.WithLibrary(LibraryId).WithBooks(3).AsPublic().ForAccount(AccountId).Build(4);

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
        public void ShouldReturnCorrectBookShelves()
        {
            var expectedItems = BookShelfBuilder.BookShelves.Where(b => b.AccountId != AccountId);
            foreach (var item in expectedItems)
            {
                var actual = _assert.Data.FirstOrDefault(x => x.Id == item.Id);
                var assert = Services.GetService<BookShelfAssert>()
                        .ForView(actual)
                        .ForLibrary(LibraryId)
                        .ShouldBeSameAs(item)
                        .WithBookCount(3)
                        .ShouldHavePublicImageLink();
                if (item.AccountId == AccountId)
                {
                    assert.WithEditableLinks();
                }
                else
                {
                    assert.WithReadOnlyLinks();
                }
            }
        }

        [Test]
        public void ShouldNotReturnBookShelvesForUser()
        {
            var bookShelvesForUser = BookShelfBuilder.BookShelves.Where(b => b.AccountId == AccountId);
            foreach (var bookShelfForUser in bookShelvesForUser)
            {
                _assert.Data.Should().NotContain(x => x.Id == bookShelfForUser.Id);
            }

        }
    }
}
