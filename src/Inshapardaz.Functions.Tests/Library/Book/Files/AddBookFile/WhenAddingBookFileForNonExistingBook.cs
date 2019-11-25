using System.Threading;
using System.Threading.Tasks;
using Bogus;
using Inshapardaz.Functions.Tests.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Functions.Tests.Library.Book.Files.AddBookFile
{
    [TestFixture]
    public class WhenAddingBookFileForNonExistingBook : FunctionTest
    {
        private BadRequestResult _response;

        [OneTimeSetUp]
        public async Task Setup()
        {

            var contents = new Faker().Random.Words(60);
            var handler = Container.GetService<Functions.Library.Books.Files.AddBookFile>();
            var request = new RequestBuilder().WithBody(contents).BuildRequestMessage();
            _response = (BadRequestResult) await handler.Run(request, Random.Number, AuthenticationBuilder.WriterClaim, CancellationToken.None);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldHaveBadRequestResult()
        {
            Assert.That(_response, Is.Not.Null);
        }
    }
}
