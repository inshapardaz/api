using System.Threading;
using System.Threading.Tasks;
using Bogus;
using Inshapardaz.Functions.Tests.Asserts;
using Inshapardaz.Functions.Tests.DataBuilders;
using Inshapardaz.Functions.Tests.Dto;
using Inshapardaz.Functions.Tests.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Functions.Tests.Library.Book.Contents.UpdateBookFile
{
    [TestFixture]
    public class WhenUpdatingBookContentAsUnauthorisedUser
        : LibraryTest<Functions.Library.Books.Content.UpdateBookContent>
    {
        private UnauthorizedResult _response;

        private BookDto _book;
        private byte[] _expected;
        private BooksDataBuilder _dataBuilder;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _dataBuilder = Container.GetService<BooksDataBuilder>();

            _book = _dataBuilder.WithLibrary(LibraryId).WithContent().Build();
            var file = _dataBuilder.Contents.PickRandom();

            _expected = new Faker().Image.Random.Bytes(50);
            var request = new RequestBuilder()
                .WithBytes(_expected)
                .WithLanguage(file.Language)
                .WithContentType(file.MimeType)
                .BuildRequestMessage();

            _response = (UnauthorizedResult)await handler.Run(request, LibraryId, _book.Id, AuthenticationBuilder.Unauthorized, CancellationToken.None);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldReturnUnauthorizedResult()
        {
            _response.ShouldBeUnauthorized();
        }
    }
}
