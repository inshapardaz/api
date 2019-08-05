﻿using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Bogus;
using Inshapardaz.Domain.Entities;
using Inshapardaz.Functions.Tests.DataBuilders;
using Inshapardaz.Functions.Tests.Helpers;
using Inshapardaz.Functions.Views.Library;
using Inshapardaz.Ports.Database.Entities.Library;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;

namespace Inshapardaz.Functions.Tests.Library.Book.UpdateBook
{
    [TestFixture]
    public class WhenUpdatingBookAsAdministrator : FunctionTest
    {
        OkObjectResult _response;
        private BooksDataBuilder _dataBuilder;
        private BookView _expected;
        private Ports.Database.Entities.Library.Author _otherAuthor;
        private IEnumerable<Category> _categories;
        private CategoriesDataBuilder _categoriesBuilder;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _dataBuilder = Container.GetService<BooksDataBuilder>();
            var authorBuilder = Container.GetService<AuthorsDataBuilder>();
            _categoriesBuilder = Container.GetService<CategoriesDataBuilder>();

            var handler = Container.GetService<Functions.Library.Books.UpdateBook>();

            _otherAuthor = authorBuilder.WithAuthors(1, 0).Build().Single();
            _categories = _categoriesBuilder.WithCategories(3).Build();
            var books = _dataBuilder.WithBooks(4, categoryCount: 1).Build();

            var selectedBook = books.First();

            var fake = new Faker();
            _expected = new BookView { 
                Title = fake.Random.String(),
                Description = fake.Random.Words(5),
                Copyrights = fake.Random.Int(0, 3),
                Language = fake.Random.Int(0, 28),
                YearPublished = fake.Date.Past().Year,
                Status = fake.Random.Int(0, 2),
                IsPublic = fake.Random.Bool(),
                AuthorId = _otherAuthor.Id,
                IsPublished = fake.Random.Bool(),
                Categories = _categories.Select(c => new CategoryView { Id = c.Id} )
            };
            _response = (OkObjectResult) await handler.Run(_expected, NullLogger.Instance, selectedBook.Id, AuthenticationBuilder.AdminClaim, CancellationToken.None);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldHaveCreatedResult()
        {
            Assert.That(_response, Is.Not.Null);
            Assert.That(_response.StatusCode, Is.EqualTo((int) HttpStatusCode.OK));
        }

        [Test]
        public void ShouldHaveUpdatedTheBook()
        {
            var returned = _response.Value as BookView;
            Assert.That(returned, Is.Not.Null);

            var actual = _dataBuilder.GetById(returned.Id);
            Assert.That(actual.Title, Is.EqualTo(_expected.Title), "Book should have expected title.");
            Assert.That(actual.Description, Is.EqualTo(_expected.Description), "Book should have expected description.");
            Assert.That(actual.AuthorId, Is.EqualTo(_otherAuthor.Id), "Author should be updated");
            Assert.That(actual.Language, Is.EqualTo((Languages)_expected.Language), "Book should have expected language.");
            Assert.That(actual.YearPublished, Is.EqualTo(_expected.YearPublished), "Book should have expected year published.");
            Assert.That(actual.Status, Is.EqualTo((BookStatuses)_expected.Status), "Book should have expected year status.");
            Assert.That(actual.IsPublic, Is.EqualTo(_expected.IsPublic), "Book should have expected year is public.");
            Assert.That(actual.IsPublished, Is.EqualTo(_expected.IsPublished), "Book should have expected year is published.");
        }

        [Test, Ignore("Fix The category assertion")]
        public void ShouldHaveUpdatedCategories()
        {
            var returned = _response.Value as BookView;
            var categories = _categoriesBuilder.GetByBookId(returned.Id);
            Assert.That(categories.Count(), Is.EqualTo(_categories.Count()), "Number of categories doesn't match");

            foreach (var category in _categories)
            {
                Assert.That(categories.SingleOrDefault(c => c.CategoryId == category.Id), Is.Not.Null, "Expected category not found in database");
            }
        }
    }
}