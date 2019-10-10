using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Bogus;
using Inshapardaz.Domain.Entities;
using Inshapardaz.Functions.Tests.DataBuilders;
using Inshapardaz.Functions.Tests.Helpers;
using Inshapardaz.Functions.Views.Library;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;

namespace Inshapardaz.Functions.Tests.Library.Book.AddBook
{
    [TestFixture]
    public class WhenAddingBookAsAdministrator : FunctionTest
    {
        private CreatedResult _response;
        private BooksDataBuilder _builder;
        private BookView _request;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _builder = Container.GetService<BooksDataBuilder>();

            var author = Container.GetService<AuthorsDataBuilder>().Build();
            var series = Container.GetService<SeriesDataBuilder>().Build();
            var categories = Container.GetService<CategoriesDataBuilder>()
                                      .Build(3)
                                      .Select(c => new CategoryView
                                      {
                                          Id = c.Id
                                      });

            var handler = Container.GetService<Functions.Library.Books.AddBook>();
            _request = new Faker<BookView>()
                       .RuleFor(c => c.Id, 0)
                       .RuleFor(c => c.Title, f => f.Random.AlphaNumeric(10))
                       .RuleFor(c => c.Description, f => f.Random.Words(10))
                       .RuleFor(c => c.Copyrights, f => f.Random.Int(0, 3))
                       .RuleFor(c => c.DateAdded, f => f.Date.Past())
                       .RuleFor(c => c.DateUpdated, f => f.Date.Past())
                       .RuleFor(c => c.Language, f => f.Random.Int(0, 28))
                       .RuleFor(c => c.IsPublic, f => f.Random.Bool())
                       .RuleFor(c => c.IsPublished, f => f.Random.Bool())
                       .RuleFor(c => c.YearPublished, f => f.Random.Int(1900, 2000))
                       .RuleFor(c => c.Status, f => f.Random.Int(0, 2))
                       .RuleFor(c => c.AuthorId, author.Id)
                       .RuleFor(c => c.SeriesId, series.Id)
                       .RuleFor(c => c.SeriesIndex, f => f.Random.Int(1, 10))
                       .RuleFor(c => c.Categories, f => f.PickRandom<CategoryView>(categories, 2))
                       .Generate();

            var request = new RequestBuilder()
                                            .WithJsonBody(_request)
                                            .Build();

            _response = (CreatedResult) await handler.Run(request, AuthenticationBuilder.AdminClaim, CancellationToken.None);
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
            Assert.That(_response.StatusCode, Is.EqualTo((int)HttpStatusCode.Created));
        }

        [Test]
        public void ShouldHaveLocationHeader()
        {
            Assert.That(_response.Location, Is.Not.Empty);
        }

        [Test]
        public void ShouldHaveCreatedTheBook()
        {
            var bookView = _response.Value as BookView;
            Assert.That(bookView, Is.Not.Null);

            var actual = _builder.GetById(bookView.Id);
            Assert.That(actual, Is.Not.Null, "Book should be created.");
            Assert.That(actual.Title, Is.EqualTo(_request.Title), "Title should match");
            Assert.That(actual.Description, Is.EqualTo(_request.Description), "Book description count does not match");
            Assert.That(actual.Language, Is.EqualTo((Languages)_request.Language), "Book language should match");
            Assert.That(actual.IsPublic, Is.EqualTo(_request.IsPublic), "Book isPublic flag should match");
            Assert.That(actual.IsPublished, Is.EqualTo(_request.IsPublished), "Book isPublished flag should match");
            Assert.That(actual.Copyrights, Is.EqualTo((CopyrightStatuses)_request.Copyrights), "Book copyrights should match");
            Assert.That(actual.DateAdded, Is.EqualTo(_request.DateAdded), "Book date added should match");
            Assert.That(actual.DateUpdated, Is.EqualTo(_request.DateUpdated), "Book date updated should match");
            Assert.That(actual.Status, Is.EqualTo((BookStatuses)_request.Status), "Book status should match");
            Assert.That(actual.YearPublished, Is.EqualTo(_request.YearPublished), "Book year published should match");
            Assert.That(actual.AuthorId, Is.EqualTo(_request.AuthorId), "Book author id should match");
            Assert.That(actual.SeriesId, Is.EqualTo(_request.SeriesId), "Book series id should match");
            Assert.That(actual.SeriesIndex, Is.EqualTo(_request.SeriesIndex), "Book series index should match");
        }
    }
}
