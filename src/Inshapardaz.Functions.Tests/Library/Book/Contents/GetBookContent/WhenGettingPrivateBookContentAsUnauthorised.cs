using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Functions.Tests.Asserts;
using Inshapardaz.Functions.Tests.DataBuilders;
using Inshapardaz.Functions.Tests.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Functions.Tests.Library.Book.Contents.GetBookContent
{
    [TestFixture]
    public class WhenGettingPrivateBookContentAsUnauthorised
        : LibraryTest<Functions.Library.Books.Content.GetBookContent>
    {
        private StatusCodeResult _response;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var dataBuilder = Container.GetService<BooksDataBuilder>();

            var book = dataBuilder.WithLibrary(LibraryId).WithContents(5).IsPublic(false).Build();
            var expected = dataBuilder.Contents.PickRandom();

            var request = new RequestBuilder().WithAccept(expected.MimeType).WithLanguage(expected.Language).Build();
            _response = (StatusCodeResult)await handler.Run(request, LibraryId, book.Id, AuthenticationBuilder.Unauthorized, CancellationToken.None);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldReturnUnauthorised()
        {
            _response.ShouldBeUnauthorized();
        }
    }
}
