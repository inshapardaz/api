﻿using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.Book.GetRecentReadBooks
{
    [TestFixture]
    public class WhenGettingRecentBooksPageInMiddle
        : TestBase
    {
        private HttpResponseMessage _response;
        private PagingAssert<BookView> _assert;
        private IEnumerable<BookDto> _books;

        public WhenGettingRecentBooksPageInMiddle()
            : base(Role.Reader)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            _books = BookBuilder.WithLibrary(LibraryId)
                                       .IsPublic()
                                       .AddToRecentReads(AccountId, 25)
                                       .Build(35);

            _response = await Client.GetAsync($"/libraries/{LibraryId}/books?pageNumber=2&pageSize=10&read=true");
            _assert = Services.GetService<PagingAssert<BookView>>().ForResponse(_response);
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
            _assert.ShouldHaveSelfLink($"/libraries/{LibraryId}/books", 2, 10,
                new KeyValuePair<string, string>("read", bool.TrueString));
        }

        [Test]
        public void ShouldHaveNextLink()
        {
            _assert.ShouldHaveNextLink($"/libraries/{LibraryId}/books", 3, 10, new KeyValuePair<string, string>("read", bool.TrueString));
        }

        [Test]
        public void ShouldHavePreviousLink()
        {
            _assert.ShouldHavePreviousLink($"/libraries/{LibraryId}/books", 1, 10, new KeyValuePair<string, string>("read", bool.TrueString));
        }

        [Test]
        public void ShouldReturnCorrectPage()
        {
            _assert.ShouldHavePage(2)
                   .ShouldHavePageSize(10)
                   .ShouldHaveTotalCount(25)
                   .ShouldHaveItems(10);
        }

        [Test]
        public void ShouldReturnExpectedBooks()
        {
            var expectedItems = BookBuilder.RecentReads.OrderBy(a => a.DateRead).Skip(10).Take(10).ToArray();
            for (int i = 0; i < _assert.Data.Count(); i++)
            {
                var actual = _assert.Data.ElementAt(i);
                var expected = _books.SingleOrDefault(b => b.Id == expectedItems[i].BookId);
                Services.GetService<BookAssert>().ForView(actual).ForLibrary(LibraryId)
                            .ShouldBeSameAs(expected)
                            .ShouldHaveCorrectLinks()
                            .ShouldNotHaveEditLinks()
                            .ShouldNotHaveImageUpdateLink()
                            .ShouldNotHaveCreateChaptersLink()
                            .ShouldNotHaveAddContentLink()
                            .ShouldHaveChaptersLink()
                            .ShouldHavePublicImageLink();
            }
        }
    }
}
