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
    public class WhenAddingBookContentAsUnauthorized
        : LibraryTest<Functions.Library.Books.Content.AddBookContent>
    {
        private UnauthorizedResult _response;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var dataBuilder = Container.GetService<BooksDataBuilder>();

            var book = dataBuilder.WithLibrary(LibraryId).Build();
            var contents = new Faker().Random.Words(60);
            var request = new RequestBuilder().WithBody(contents).BuildRequestMessage();
            _response = (UnauthorizedResult)await handler.Run(request, LibraryId, book.Id, AuthenticationBuilder.Unauthorized, CancellationToken.None);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldHaveUnauthorizedResult()
        {
            _response.ShouldBeUnauthorized();
        }
    }
}
