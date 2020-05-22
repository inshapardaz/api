using System.Linq;
using System.Security.Claims;
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

namespace Inshapardaz.Functions.Tests.Library.Chapter.Contents.DeleteChapterContents
{
    [TestFixture(AuthenticationLevel.Administrator)]
    [TestFixture(AuthenticationLevel.Writer)]
    public class WhenDeletingChapterContentsWithPermission
        : LibraryTest<Functions.Library.Books.Chapters.Contents.DeleteChapterContents>
    {
        private NoContentResult _response;
        private ChapterContentDto _content;
        private ClaimsPrincipal _claim;

        public WhenDeletingChapterContentsWithPermission(AuthenticationLevel authenticationLevel)
        {
            _claim = AuthenticationBuilder.CreateClaim(authenticationLevel);
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var _dataBuilder = Container.GetService<ChapterDataBuilder>();
            var _newContents = new Faker().Random.Words(12);
            var chapter = _dataBuilder.WithLibrary(LibraryId).WithContents().Build();
            _content = _dataBuilder.Contents.Single(x => x.ChapterId == chapter.Id);
            var file = _dataBuilder.Files.Single(x => x.Id == _content.FileId);

            var request = new RequestBuilder().WithBody(_newContents)
                                              .WithLanguage(_content.Language)
                                              .WithContentType(file.MimeType)
                                              .Build();

            _response = (NoContentResult)await handler.Run(request, LibraryId, chapter.BookId, chapter.Id, _claim, CancellationToken.None);
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
        public void ShouldHaveDeletedChapterContent()
        {
            ChapterContentAssert.ShouldHaveDeletedContent(DatabaseConnection, _content);
        }
    }
}
