﻿using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.Book.GetRecentReadBooks
{
    [TestFixture]
    public class WhenGettingRecentBooksPageThatDoesNotExist
        : TestBase
    {
        private HttpResponseMessage _response;
        private PagingAssert<BookView> _assert;

        public WhenGettingRecentBooksPageThatDoesNotExist()
            : base(Role.Reader)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            BookBuilder.WithLibrary(LibraryId)
                                       .IsPublic()
                                       .AddToRecentReads(AccountId, 10)
                                       .Build(25);

            _response = await Client.GetAsync($"/libraries/{LibraryId}/books?pageNumber=3&pageSize=10&read=true");
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
            _assert.ShouldHaveSelfLink($"/libraries/{LibraryId}/books", 3, 10,
                new KeyValuePair<string, string>("read", bool.TrueString));
        }

        [Test]
        public void ShouldNotHaveNextLink()
        {
            _assert.ShouldNotHaveNextLink();
        }

        [Test]
        public void ShouldNotHavePreviousLink()
        {
            _assert.ShouldNotHavePreviousLink();
        }

        [Test]
        public void ShouldReturnCorrectPage()
        {
            _assert.ShouldHavePage(3)
                   .ShouldHavePageSize(10)
                   .ShouldHaveTotalCount(10)
                   .ShouldHaveItems(0);
        }

        [Test]
        public void ShouldReturnExpectedBooks()
        {
            _assert.ShouldHaveNoData();
        }
    }
}
