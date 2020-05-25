using System.Threading;
using System.Threading.Tasks;
using Bogus;
using Inshapardaz.Domain.Repositories;
using Inshapardaz.Functions.Tests.Asserts;
using Inshapardaz.Functions.Tests.DataBuilders;
using Inshapardaz.Functions.Tests.Fakes;
using Inshapardaz.Functions.Tests.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Functions.Tests.Library.Chapter.Contents.GetChapterContentsById
{
    [TestFixture]
    public class WhenGettingChapterContentsByIdWhenNoChapterNotExists
        : LibraryTest<Functions.Library.Books.Chapters.Contents.GetChapterContentsById>
    {
        private NotFoundResult _response;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var request = TestHelpers.CreateGetRequest();

            var dataBuilder = Container.GetService<BooksDataBuilder>();
            var book = dataBuilder.WithLibrary(LibraryId).Build();

            _response = (NotFoundResult)await handler.Run(request, LibraryId, book.Id, -Random.Number, -Random.Number, AuthenticationBuilder.ReaderClaim, CancellationToken.None);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldHaveNotFoundResult()
        {
            _response.ShouldBeNotFound();
        }
    }
}
