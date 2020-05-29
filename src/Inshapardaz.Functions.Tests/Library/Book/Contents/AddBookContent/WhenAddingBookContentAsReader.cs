using System.Threading;
using System.Threading.Tasks;
using Bogus;
using Inshapardaz.Functions.Tests.Asserts;
using Inshapardaz.Functions.Tests.DataBuilders;
using Inshapardaz.Functions.Tests.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Functions.Tests.Library.Book.Contents.AddBookContent
{
    [TestFixture]
    public class WhenAddingBookContentAsReader
        : LibraryTest<Functions.Library.Books.Content.AddBookContent>
    {
        private ForbidResult _response;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var dataBuilder = Container.GetService<BooksDataBuilder>();

            var book = dataBuilder.WithLibrary(LibraryId).Build();
            var contents = new Faker().Random.Words(60);
            var request = new RequestBuilder().WithBody(contents).BuildRequestMessage();
            _response = (ForbidResult)await handler.Run(request, LibraryId, book.Id, AuthenticationBuilder.ReaderClaim, CancellationToken.None);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldHaveForbidResult()
        {
            _response.ShouldBeForbidden();
        }
    }
}
