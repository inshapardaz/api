using System.Threading;
using System.Threading.Tasks;
using Bogus;
using Inshapardaz.Functions.Tests.Asserts;
using Inshapardaz.Functions.Tests.Helpers;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;

namespace Inshapardaz.Functions.Tests.Library.Book.Contents.AddBookContent
{
    [TestFixture]
    public class WhenAddingBookContentForNonExistingBook
        : LibraryTest<Functions.Library.Books.Content.AddBookContent>
    {
        private BadRequestResult _response;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var contents = new Faker().Random.Words(60);
            var request = new RequestBuilder().WithBody(contents).BuildRequestMessage();
            _response = (BadRequestResult)await handler.Run(request, LibraryId, Random.Number, AuthenticationBuilder.WriterClaim, CancellationToken.None);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldHaveBadRequestResult()
        {
            _response.ShouldBeBadRequest();
        }
    }
}
