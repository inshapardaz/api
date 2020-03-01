using System.Threading;
using System.Threading.Tasks;
using Bogus;
using Inshapardaz.Functions.Tests.DataBuilders;
using Inshapardaz.Functions.Tests.Helpers;
using Inshapardaz.Functions.Views.Library;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Functions.Tests.Library.Author.AddAuthor
{
    [TestFixture]
    public class WhenAddingAuthorAsReader : LibraryTest
    {
        private ForbidResult _response;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var handler = Container.GetService<Functions.Library.Authors.AddAuthor>();
            var author = new AuthorView { Name = new Faker().Random.String() };

            var request = new RequestBuilder()
                                            .WithJsonBody(author)
                                            .Build();
            _response = (ForbidResult)await handler.Run(request, LibraryId, AuthenticationBuilder.ReaderClaim, CancellationToken.None);
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
