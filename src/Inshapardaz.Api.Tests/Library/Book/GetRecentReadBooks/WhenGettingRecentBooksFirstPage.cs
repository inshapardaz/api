﻿using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.Dto;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Models;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.Book.GetRecentReadBooks
{
    [TestFixture]
    public class WhenGettingRecentBooksFirstPage
        : TestBase
    {
        private HttpResponseMessage _response;
        private PagingAssert<BookView> _assert;
        private IEnumerable<BookDto> _books;

        public WhenGettingRecentBooksFirstPage()
            : base(Role.Reader)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            _books = BookBuilder.WithLibrary(LibraryId)
                                       .IsPublic()
                                       .AddToRecentReads(AccountId, 15)
                                       .Build(25);

            _response = await Client.GetAsync($"/libraries/{LibraryId}/books?pageNumber=1&pageSize=10&read=true");
            _assert = new PagingAssert<BookView>(_response);
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
            _assert.ShouldHaveSelfLink($"/libraries/{LibraryId}/books", 1, 10,
                new KeyValuePair<string, string>("read", bool.TrueString));
        }

        [Test]
        public void ShouldHaveNextLink()
        {
            _assert.ShouldHaveNextLink($"/libraries/{LibraryId}/books", 2, 10,
                new KeyValuePair<string, string>("read", bool.TrueString));
        }

        [Test]
        public void ShouldNotHavePreviousLink()
        {
            _assert.ShouldNotHavePreviousLink();
        }

        [Test]
        public void ShouldReturnCorrectPage()
        {
            _assert.ShouldHavePage(1)
                   .ShouldHavePageSize(10)
                   .ShouldHaveTotalCount(15)
                   .ShouldHaveItems(10);
        }

        [Test]
        public void ShouldReturnExpectedBooks()
        {
            var expectedItems = BookBuilder.RecentReads.OrderBy(a => a.DateRead).Take(10).ToArray();
            for (int i = 0; i < _assert.Data.Count(); i++)
            {
                var actual = _assert.Data.ElementAt(i);
                var expected = _books.SingleOrDefault(b => b.Id == expectedItems[i].BookId);
                actual.ShouldMatch(expected, DatabaseConnection, LibraryId)
                            .InLibrary(LibraryId)
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
