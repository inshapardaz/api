using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Functions.Tests.Asserts;
using Inshapardaz.Functions.Tests.DataBuilders;
using Inshapardaz.Functions.Tests.DataHelpers;
using Inshapardaz.Functions.Tests.Dto;
using Inshapardaz.Functions.Tests.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;

namespace Inshapardaz.Functions.Tests.Library.Chapter.DeleteChapter
{
    [TestFixture(AuthenticationLevel.Administrator)]
    [TestFixture(AuthenticationLevel.Writer)]
    public class WhenDeletingChapterWithPermission
        : LibraryTest<Functions.Library.Books.Chapters.DeleteChapter>
    {
        private readonly ClaimsPrincipal _claim;

        private NoContentResult _response;

        private ChapterDto _expected;
        private ChapterDataBuilder _dataBuilder;

        public WhenDeletingChapterWithPermission(AuthenticationLevel authenticationLevel)
        {
            _claim = AuthenticationBuilder.CreateClaim(authenticationLevel);
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var request = TestHelpers.CreateGetRequest();
            _dataBuilder = Container.GetService<ChapterDataBuilder>();
            _expected = _dataBuilder.WithLibrary(LibraryId).WithContents().Build();

            _response = (NoContentResult)await handler.Run(request, LibraryId, _expected.BookId, _expected.Id, _claim, NullLogger.Instance, CancellationToken.None);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldReturnNoContent()
        {
            _response.ShouldBeNoContent();
        }

        [Test]
        public void ShouldHaveDeletedChapter()
        {
            ChapterAssert.ShouldHaveDeletedChapter(_expected.Id, DatabaseConnection);
        }

        [Test]
        public void ShouldHaveDeletedTheChapterContents()
        {
            ChapterAssert.ThatFilesAreDeletedForChapter(_expected.Id, DatabaseConnection);
        }
    }
}
