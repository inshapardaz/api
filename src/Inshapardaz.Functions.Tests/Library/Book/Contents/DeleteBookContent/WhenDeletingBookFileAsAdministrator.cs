using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Functions.Tests.Asserts;
using Inshapardaz.Functions.Tests.DataBuilders;
using Inshapardaz.Functions.Tests.Dto;
using Inshapardaz.Functions.Tests.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Functions.Tests.Library.Book.Contents.DeleteBookContent
{
    [TestFixture(AuthenticationLevel.Administrator)]
    [TestFixture(AuthenticationLevel.Writer)]
    public class WhenDeletingBookFileWithPermissions
        : LibraryTest<Functions.Library.Books.Content.DeleteBookContent>
    {
        private NoContentResult _response;

        private BookContentDto _expected;
        private BooksDataBuilder _dataBuilder;
        private ClaimsPrincipal _claim;

        public WhenDeletingBookFileWithPermissions(AuthenticationLevel authenticationLevel)
        {
            _claim = AuthenticationBuilder.CreateClaim(authenticationLevel);
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            _dataBuilder = Container.GetService<BooksDataBuilder>();
            var book = _dataBuilder.WithLibrary(LibraryId).WithContents(3).Build();
            _expected = _dataBuilder.Contents.PickRandom();

            var request = new RequestBuilder().WithLanguage(_expected.Language).WithAccept(_expected.MimeType).Build();
            _response = (NoContentResult)await handler.Run(request, LibraryId, _expected.BookId, _claim, CancellationToken.None);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldHaveNoContentResult()
        {
            _response.ShouldBeNoContent();
        }

        [Test]
        public void ShouldHaveDeletedBookFile()
        {
            BookContentAssert.ShouldNotHaveBookContent(_expected.BookId, _expected.Language, _expected.MimeType, DatabaseConnection);
        }

        [Test]
        public void ShouldNotHaveDeletedOtherBookFiles()
        {
            foreach (var item in _dataBuilder.Contents)
            {
                if (item.Id == _expected.Id)
                {
                    continue;
                }

                BookContentAssert.ShouldHaveBookContent(item.BookId, item.Language, item.MimeType, DatabaseConnection);
            }
        }
    }
}
