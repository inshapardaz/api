﻿using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Models;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.Book.GetBooks
{
    [TestFixture]
    public class WhenGettingBooksWithMultipleAuthorsAndCategories : TestBase
    {
        private HttpResponseMessage _response;
        private PagingAssert<BookView> _assert;
        private IEnumerable<CategoryDto> _categories;
        private IEnumerable<AuthorDto> _authors;

        public WhenGettingBooksWithMultipleAuthorsAndCategories() : base(Role.Reader)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            _categories = CategoryBuilder.WithLibrary(LibraryId).Build(2);
            _authors = AuthorBuilder.WithLibrary(LibraryId).Build(2);

            BookBuilder.WithLibrary(LibraryId).WithCategories(_categories).WithAuthors(_authors).HavingSeries().WithContents(2).Build(40);

            _response = await Client.GetAsync($"/libraries/{LibraryId}/books?pageNumber=2&pageSize=10");

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
            _assert.ShouldHaveSelfLink($"/libraries/{LibraryId}/books");
        }

        [Test]
        public void ShouldNotHaveCreateLink()
        {
            _assert.ShouldNotHaveCreateLink();
        }

        [Test]
        public void ShouldHaveNextLink()
        {
            _assert.ShouldHaveNextLink($"/libraries/{LibraryId}/books", 3);
        }

        [Test]
        public void ShouldHavePreviousLink()
        {
            _assert.ShouldHavePreviousLink($"/libraries/{LibraryId}/books", 1);
        }

        [Test]
        public void ShouldReturnExpectedBooks()
        {
            var expectedItems = BookBuilder.Books.OrderBy(a => a.Title).Skip(10).Take(10).ToArray();
            for (int i = 0; i < _assert.Data.Count(); i++)
            {
                var actual = _assert.Data.ElementAt(i);
                var expected = expectedItems[i];
                actual.ShouldMatch(expected, DatabaseConnection, LibraryId)
                            .InLibrary(LibraryId)
                            .ShouldHaveCorrectLinks()
                            .ShouldNotHaveEditLinks()
                            .ShouldNotHaveImageUpdateLink()
                            .ShouldNotHaveCreateChaptersLink()
                            .ShouldNotHaveAddContentLink()
                            .ShouldHaveSeriesLink()
                            .ShouldBeSameCategories(_categories)
                            .ShouldHaveChaptersLink()
                            .ShouldHavePublicImageLink()
                            .ShouldHaveAddFavoriteLink()
                            .ShouldHaveContents(DatabaseConnection, false);
            }
        }
    }
}
