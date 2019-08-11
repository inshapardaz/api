using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Bogus;
using Inshapardaz.Functions.Tests.DataBuilders;
using Inshapardaz.Functions.Tests.Helpers;
using Inshapardaz.Functions.Views.Library;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;

namespace Inshapardaz.Functions.Tests.Library.Book.UpdateBook
{
    [TestFixture]
    public class WhenUpdatingBookThatDoesNotExist : FunctionTest
    {
        CreatedResult _response;
        private BooksDataBuilder _builder;
        private BookView _expected;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _builder = Container.GetService<BooksDataBuilder>();
            var author = Container.GetService<AuthorsDataBuilder>().Build();

            var handler = Container.GetService<Functions.Library.Books.UpdateBook>();
            var faker = new Faker();
            _expected = new BookView
            {
                Title = faker.Random.String(),
                Description = faker.Random.String(),
                AuthorId = author.Id
            };
            _response = (CreatedResult) await handler.Run(_expected, NullLogger.Instance, _expected.Id, AuthenticationBuilder.AdminClaim, CancellationToken.None);
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
            var returned = _response.Value as BookView;
            Assert.That(returned, Is.Not.Null);

            var actual = _builder.GetById(returned.Id);
            Assert.That(actual, Is.Not.Null, "Book should be created.");
        }
    }
}
