using System.Threading;
using System.Threading.Tasks;
using Bogus;
using Inshapardaz.Functions.Tests.Helpers;
using Inshapardaz.Functions.Views.Library;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Functions.Tests.Library.Book.UpdateBook
{
    [TestFixture]
    public class WhenUpdatingBookAsUnauthorized : LibraryTest
    {
        private UnauthorizedResult _response;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var handler = Container.GetService<Functions.Library.Books.UpdateBook>();
            var faker = new Faker();
            var book = new BookView { Id = faker.Random.Number(), Title = faker.Random.String() };
            var request = new RequestBuilder()
                                            .WithJsonBody(book)
                                            .Build();
            _response = (UnauthorizedResult)await handler.Run(request, LibraryId, book.Id, AuthenticationBuilder.Unauthorized, CancellationToken.None);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldHaveForbiddenResult()
        {
            Assert.That(_response, Is.Not.Null);
        }
    }
}
