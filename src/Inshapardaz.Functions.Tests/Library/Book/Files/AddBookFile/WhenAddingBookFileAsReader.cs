using System.Threading;
using System.Threading.Tasks;
using Bogus;
using Inshapardaz.Functions.Tests.DataBuilders;
using Inshapardaz.Functions.Tests.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Functions.Tests.Library.Book.Files.AddBookFile
{
    [TestFixture]
    public class WhenAddingBookFileAsReader : FunctionTest
    {
        private ForbidResult _response;
        private BooksDataBuilder _dataBuilder;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _dataBuilder = Container.GetService<BooksDataBuilder>();

            var book = _dataBuilder.Build();
            var contents = new Faker().Random.Words(60);
            var handler = Container.GetService<Functions.Library.Books.Files.AddBookFile>();
            var request = new RequestBuilder().WithBody(contents).BuildRequestMessage();
            _response = (ForbidResult) await handler.Run(request, book.Id, AuthenticationBuilder.ReaderClaim, CancellationToken.None);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldHaveForbidResult()
        {
            Assert.That(_response, Is.Not.Null);
        }
    }
}
