using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Functions.Tests.Asserts;
using Inshapardaz.Functions.Tests.Helpers;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;

namespace Inshapardaz.Functions.Tests.Library.Chapter.GetChapterById
{
    [TestFixture]
    public class WhenGettingChapterByIdThatDoesNotExist
        : LibraryTest<Functions.Library.Books.Chapters.GetChapterById>
    {
        private NotFoundResult _response;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _response = (NotFoundResult)await handler.Run(LibraryId, Random.Number, Random.Number, AuthenticationBuilder.WriterClaim, CancellationToken.None);
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
