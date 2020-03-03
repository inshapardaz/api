using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Bogus;
using Inshapardaz.Functions.Tests.DataBuilders;
using Inshapardaz.Functions.Tests.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Functions.Tests.Library.Book.Files.UpdateBookFile
{
    [TestFixture]
    public class WhenUpdatingBookFileAsUnauthorisedUser : LibraryTest<Functions.Library.Books.Files.UpdateBookFile>
    {
        private UnauthorizedResult _response;

        private Ports.Database.Entities.Library.Book _book;
        private byte[] _expected;
        private BooksDataBuilder _dataBuilder;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _dataBuilder = Container.GetService<BooksDataBuilder>();

            _book = _dataBuilder.WithFile().Build();
            _expected = new Faker().Image.Random.Bytes(50);
            var request = new RequestBuilder().WithBytes(_expected).BuildRequestMessage();
            _response = (UnauthorizedResult)await handler.Run(request, LibraryId, _book.Id, _book.Files.First().Id, AuthenticationBuilder.Unauthorized, CancellationToken.None);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldReturnUnauthorizedResult()
        {
            Assert.That(_response, Is.Not.Null);
        }
    }
}
