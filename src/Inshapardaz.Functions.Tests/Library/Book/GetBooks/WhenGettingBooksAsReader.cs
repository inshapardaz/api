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

namespace Inshapardaz.Functions.Tests.Library.Book.GetBooks
{
    [TestFixture]
    public class WhenGettingBooksAsReader
        : LibraryTest<Functions.Library.Books.GetBooks>
    {
        private OkObjectResult _response;
        private PagingAssert<BookView> _assert;
        private BooksDataBuilder _builder;
        private IEnumerable<CategoryDto> _categories;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var request = TestHelpers.CreateGetRequest();

            _builder = Container.GetService<BooksDataBuilder>();
            _categories = Container.GetService<CategoriesDataBuilder>().WithLibrary(LibraryId).Build(2);

            _builder.WithLibrary(LibraryId).WithCategories(_categories).HavingSeries().WithContents(2).Build(4);

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
            _assert.ShouldHaveSelfLink($"/api/library/{LibraryId}/books");
        }

        [Test]
        public void ShouldNotHaveCreateLink()
        {
            _assert.ShouldNotHaveCreateLink();
        }

        [Test]
        public void ShouldNotHaveNavigationLinks()
        {
            _assert.ShouldNotHaveNextLink();
            _assert.ShouldNotHavePreviousLink();
        }

        [Test]
        public void ShouldReturnExpectedBooks()
        {
            var expectedItems = _builder.Books.OrderBy(a => a.Title).Take(10);
            foreach (var item in expectedItems)
            {
                var actual = _assert.Data.FirstOrDefault(x => x.Id == item.Id);
                actual.ShouldMatch(item, DatabaseConnection)
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
