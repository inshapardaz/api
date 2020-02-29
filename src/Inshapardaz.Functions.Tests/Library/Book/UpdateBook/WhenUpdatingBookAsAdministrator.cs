using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Bogus;
using Inshapardaz.Domain.Models;
using Inshapardaz.Functions.Tests.DataBuilders;
using Inshapardaz.Functions.Tests.DataHelpers;
using Inshapardaz.Functions.Tests.Dto;
using Inshapardaz.Functions.Tests.Helpers;
using Inshapardaz.Functions.Views.Library;
using Inshapardaz.Ports.Database.Entities.Library;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Functions.Tests.Library.Book.UpdateBook
{
    [TestFixture]
    public class WhenUpdatingBookAsAdministrator : FunctionTest
    {
        private OkObjectResult _response;
        private BooksDataBuilder _dataBuilder;
        private BookView _expected;
        private BookView _actual;
        private AuthorDto _otherAuthor;
        private IEnumerable<CategoryDto> _categories;
        private CategoriesDataBuilder _categoriesBuilder;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _dataBuilder = Container.GetService<BooksDataBuilder>();
            var authorBuilder = Container.GetService<AuthorsDataBuilder>();
            _categoriesBuilder = Container.GetService<CategoriesDataBuilder>();

            var handler = Container.GetService<Functions.Library.Books.UpdateBook>();

            _otherAuthor = authorBuilder.Build();
            _categories = _categoriesBuilder.Build(3);
            var books = _dataBuilder.WithCategories(1).Build(4);

            var selectedBook = books.First();

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
                AuthorId = _otherAuthor.Id,
                IsPublished = fake.Random.Bool(),
                Categories = _categories.Select(c => new CategoryView { Id = c.Id })
            };

            var request = new RequestBuilder()
                                            .WithJsonBody(_expected)
                                            .Build();

            _response = (OkObjectResult)await handler.Run(request, selectedBook.Id, AuthenticationBuilder.AdminClaim, CancellationToken.None);
            _actual = _response.Value as BookView;
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
            Assert.That(_response.StatusCode, Is.EqualTo((int)HttpStatusCode.OK));
        }

        [Test]
        public void ShouldHaveUpdatedTheBook()
        {
            Assert.That(_actual, Is.Not.Null);

            var actual = _dataBuilder.GetById(_actual.Id);
            Assert.That(actual.Title, Is.EqualTo(_expected.Title), "Book should have expected title.");
            Assert.That(actual.Description, Is.EqualTo(_expected.Description), "Book should have expected description.");
            Assert.That(actual.AuthorId, Is.EqualTo(_otherAuthor.Id), "Author should be updated");
            Assert.That(actual.Language, Is.EqualTo((Languages)_expected.Language), "Book should have expected language.");
            Assert.That(actual.YearPublished, Is.EqualTo(_expected.YearPublished), "Book should have expected year published.");
            Assert.That(actual.Status, Is.EqualTo((BookStatuses)_expected.Status), "Book should have expected year status.");
            Assert.That(actual.IsPublic, Is.EqualTo(_expected.IsPublic), "Book should have expected year is public.");
            Assert.That(actual.IsPublished, Is.EqualTo(_expected.IsPublished), "Book should have expected year is published.");
        }

        [Test]
        public void ShouldReturnCorrectCategories()
        {
            Assert.That(_actual.Categories.Count(), Is.EqualTo(_expected.Categories.Count()), "Number of categories doesn't match");
            Assert.That(_actual.Categories.Select(c => c.Id), Is.EquivalentTo(_expected.Categories.Select(x => x.Id)));
        }

        [Test]
        public void ShouldHaveUpdatedCategories()
        {
            var categories = DatabaseConnection.GetCategoriesByBook(_actual.Id);
            Assert.That(categories.Count(), Is.EqualTo(_expected.Categories.Count()), "Number of categories doesn't match");

            Assert.That(categories.Select(c => c.Id), Is.EquivalentTo(_expected.Categories.Select(x => x.Id)));
        }
    }
}
