using System.Threading;
using System.Threading.Tasks;
using Bogus;
using Inshapardaz.Functions.Tests.DataBuilders;
using Inshapardaz.Functions.Tests.Helpers;
using Inshapardaz.Functions.Views.Library;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Functions.Tests.Library.Author.UpdateAuthor
{
    [TestFixture]
    public class WhenUpdatingAuthorAsUnauthorized : LibraryTest
    {
        private UnauthorizedResult _response;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var handler = Container.GetService<Functions.Library.Authors.UpdateAuthor>();
            var faker = new Faker();
            var author = new AuthorView { Id = faker.Random.Number(), Name = faker.Random.String() };
            var request = new RequestBuilder()
                                            .WithJsonBody(author)
                                            .Build();
            _response = (UnauthorizedResult)await handler.Run(request, LibraryId, author.Id, AuthenticationBuilder.Unauthorized, CancellationToken.None);
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
