using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Bogus;
using Inshapardaz.Functions.Tests.Asserts;
using Inshapardaz.Functions.Tests.DataBuilders;
using Inshapardaz.Functions.Tests.Dto;
using Inshapardaz.Functions.Tests.Helpers;
using Inshapardaz.Functions.Views.Library;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Functions.Tests.Library.Book.UpdateBook
{
    [TestFixture(AuthenticationLevel.Administrator)]
    [TestFixture(AuthenticationLevel.Writer)]
    public class WhenUpdatingBookWithPermissions : LibraryTest<Functions.Library.Books.UpdateBook>
    {
        private readonly ClaimsPrincipal _claim;
        private OkObjectResult _response;
        private BookView _expected;
        private BookAssert _bookAssert;
        private IEnumerable<CategoryDto> _otherCategories;

        public WhenUpdatingBookWithPermissions(AuthenticationLevel authenticationLevel)
        {
            _claim = AuthenticationBuilder.CreateClaim(authenticationLevel);
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var dataBuilder = Container.GetService<BooksDataBuilder>();
            var authorBuilder = Container.GetService<AuthorsDataBuilder>();

            var otherAuthor = authorBuilder.WithLibrary(LibraryId).Build();
            _otherCategories = Container.GetService<CategoriesDataBuilder>().WithLibrary(LibraryId).Build(3);
            var otherSeries = Container.GetService<SeriesDataBuilder>().WithLibrary(LibraryId).Build();
            var books = dataBuilder.WithLibrary(LibraryId)
                                    .WithCategories(1)
                                    .HavingSeries()
                                    .AddToFavorites(Guid.NewGuid())
                                    .AddToRecentReads(Guid.NewGuid())
                                    .Build(1);

            var selectedBook = books.PickRandom();

            var fake = new Faker();
            _expected = new BookView
            {
                Title = fake.Name.FullName(),
                Description = fake.Random.Words(5),
                Copyrights = fake.Random.Int(0, 3),
                Language = fake.Random.Int(0, 28),
                YearPublished = fake.Date.Past().Year,
                Status = fake.Random.Int(0, 2),
                IsPublic = fake.Random.Bool(),
                AuthorId = otherAuthor.Id,
                SeriesId = otherSeries.Id,
                IsPublished = fake.Random.Bool(),
                Categories = _otherCategories.Select(c => new CategoryView { Id = c.Id })
            };

            _response = (OkObjectResult)await handler.Run(_expected, LibraryId, selectedBook.Id, _claim, CancellationToken.None);
            _bookAssert = BookAssert.WithResponse(_response).InLibrary(LibraryId);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldHaveOKResult()
        {
            _response.ShouldBeOk();
        }

        [Test]
        public void ShouldHaveUpdatedTheBook()
        {
            _bookAssert.ShouldBeSameAs(_expected, DatabaseConnection);
        }

        [Test]
        public void ShouldReturnCorrectCategories()
        {
            _bookAssert.ShouldBeSameCategories(_otherCategories);
        }
    }
}
