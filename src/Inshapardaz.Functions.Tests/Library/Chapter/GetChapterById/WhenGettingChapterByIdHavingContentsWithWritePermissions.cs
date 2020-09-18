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

namespace Inshapardaz.Functions.Tests.Library.Chapter.GetChapterById
{
    [TestFixture(Permission.Administrator)]
    [TestFixture(Permission.Writer)]
    public class WhenGettingChapterByIdHavingContentsWithWritePermissions
        : LibraryTest<Functions.Library.Books.Chapters.GetChapterById>
    {
        private HttpResponseMessage _response;
        private ChapterDto _expected;
        private ChapterAssert _assert;
        private ClaimsPrincipal _claim;
        private ChapterDataBuilder _dataBuilder;

        public WhenGettingChapterByIdHavingContentsWithWritePermissions(Permission Permission)
        {
            _claim = AuthenticationBuilder.CreateClaim(Permission);
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var request = new RequestBuilder().Build();
            _dataBuilder = Container.GetService<ChapterDataBuilder>();
            var chapters = _dataBuilder.WithLibrary(LibraryId).WithContents(2).Build(4);
            _expected = chapters.PickRandom();

            _response = (HttpResponseMessage)await handler.Run(request, LibraryId, _expected.BookId, _expected.Id, _claim, CancellationToken.None);

            _assert = ChapterAssert.FromResponse(_response, LibraryId);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldHaveOkResult()
        {
            _response.ShouldBeOk();
        }

        [Test]
        public void ShouldHaveCorrectObjectRetured()
        {
            _assert.ShouldMatch(_expected);
        }

        [Test]
        public void ShouldHaveLinks()
        {
            _assert.ShouldHaveSelfLink()
                   .ShouldHaveBookLink();
        }

        [Test]
        public void ShouldHaveEditLinks()
        {
            _assert.ShouldHaveUpdateLink()
                   .ShouldHaveDeleteLink()
                   .ShouldHaveAddChapterContentLink();
        }

        [Test]
        public void ShouldHaveCorrectContents()
        {
            _assert.ShouldHaveCorrectContents(DatabaseConnection);
        }
    }
}