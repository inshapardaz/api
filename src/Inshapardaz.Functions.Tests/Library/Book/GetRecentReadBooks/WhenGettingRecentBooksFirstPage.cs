﻿using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Functions.Tests.Asserts;
using Inshapardaz.Functions.Tests.DataBuilders;
using Inshapardaz.Functions.Tests.Dto;
using Inshapardaz.Functions.Tests.Helpers;
using Inshapardaz.Functions.Views.Library;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Functions.Tests.Library.Book.GetRecentReadBooks
{
    [TestFixture]
    public class WhenGettingRecentBooksFirstPage
        : LibraryTest<Functions.Library.Books.GetBooks>
    {
        private OkObjectResult _response;
        private PagingAssert<BookView> _assert;
        private BooksDataBuilder _bookBuilder;
        private IEnumerable<BookDto> _books;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var claim = AuthenticationBuilder.ReaderClaim;
            _bookBuilder = Container.GetService<BooksDataBuilder>();
            _books = _bookBuilder.WithLibrary(LibraryId)
                                       .IsPublic()
                                       .AddToRecentReads(claim.GetUserId(), 15)
                                       .Build(25);

            var request = new RequestBuilder()
                          .WithQueryParameter("pageNumber", 1)
                          .WithQueryParameter("pageSize", 10)
                          .WithQueryParameter("read", bool.TrueString)
                          .Build();

            _response = (OkObjectResult)await handler.Run(request, LibraryId, claim, CancellationToken.None);

            _assert = new PagingAssert<BookView>(_response, Library);
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
            _assert.ShouldHaveSelfLink($"/api/library/{LibraryId}/books", "pageNumber", "1");
            _assert.ShouldHaveSelfLink($"/api/library/{LibraryId}/books", "pageSize", "10");
            _assert.ShouldHaveSelfLink($"/api/library/{LibraryId}/books", "read", bool.TrueString);
        }

        [Test]
        public void ShouldHaveNextLink()
        {
            _assert.ShouldHaveNextLink($"/api/library/{LibraryId}/books", 2, 10, "read", bool.TrueString);
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
            var expectedItems = _bookBuilder.RecentReads.OrderBy(a => a.DateRead).Take(10);
            foreach (var item in expectedItems)
            {
                var actual = _assert.Data.FirstOrDefault(x => x.Id == item.BookId);
                var expected = _books.SingleOrDefault(b => b.Id == item.BookId);
                actual.ShouldMatch(expected, DatabaseConnection)
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