﻿using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Functions.Tests.Asserts;
using Inshapardaz.Functions.Tests.DataBuilders;
using Inshapardaz.Functions.Tests.Dto;
using Inshapardaz.Functions.Tests.Helpers;
using Inshapardaz.Functions.Views;
using Inshapardaz.Functions.Views.Library;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Functions.Tests.Library.Book.GetBooksByCategory
{
    [TestFixture]
    public class WhenGettingBooksByCategoryPageThatDoesNotExist
        : LibraryTest<Functions.Library.Books.GetBooks>
    {
        private OkObjectResult _response;
        private PagingAssert<BookView> _assert;
        private CategoriesDataBuilder _categoryBuilder;
        private CategoryDto _category;
        private IEnumerable<BookDto> _categoryBooks;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _categoryBuilder = Container.GetService<CategoriesDataBuilder>();
            var _bookBuilder = Container.GetService<BooksDataBuilder>();
            _category = _categoryBuilder.WithLibrary(LibraryId).Build();
            _categoryBooks = _bookBuilder.WithLibrary(LibraryId).WithCategory(_category).IsPublic().Build(25);
            _categoryBuilder.WithLibrary(LibraryId).WithBooks(3).Build();

            var request = new RequestBuilder()
                          .WithQueryParameter("pageNumber", 4)
                          .WithQueryParameter("pageSize", 10)
                          .WithQueryParameter("categoryid", _category.Id)
                          .Build();

            _response = (OkObjectResult)await handler.Run(request, LibraryId, AuthenticationBuilder.ReaderClaim, CancellationToken.None);

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
            _assert.ShouldHaveSelfLink($"/api/library/{LibraryId}/books", 4, parameterName: "categoryid", parameterValue: _category.Id.ToString());
        }

        [Test]
        public void ShouldNotHaveCreateLink()
        {
            _assert.ShouldNotHaveCreateLink();
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
            _assert.ShouldHavePage(4)
                   .ShouldHavePageSize(10)
                   .ShouldHaveTotalCount(_categoryBooks.Count())
                   .ShouldHaveItems(0);
        }

        [Test]
        public void ShouldReturnNoBooks()
        {
            _assert.ShouldHaveNoData();
        }
    }
}
