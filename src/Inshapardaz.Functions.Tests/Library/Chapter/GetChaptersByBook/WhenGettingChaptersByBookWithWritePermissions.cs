using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Inshapardaz.Functions.Tests.Asserts;
using Inshapardaz.Functions.Tests.DataBuilders;
using Inshapardaz.Functions.Tests.DataHelpers;
using Inshapardaz.Functions.Tests.Dto;
using Inshapardaz.Functions.Tests.Helpers;
using Inshapardaz.Functions.Views;
using Inshapardaz.Functions.Views.Library;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Functions.Tests.Library.Chapter.GetChaptersByBook
{
    [TestFixture(AuthenticationLevel.Administrator)]
    [TestFixture(AuthenticationLevel.Writer)]
    public class WhenGettingChaptersByBookWithWritePermissions
        : LibraryTest<Functions.Library.Books.Chapters.GetChaptersByBook>
    {
        private OkObjectResult _response;
        private ListView<ChapterView> _view;
        private readonly ClaimsPrincipal _claim;
        private ChapterDataBuilder _dataBuilder;
        private BookDto _book;

        public WhenGettingChaptersByBookWithWritePermissions(AuthenticationLevel authenticationLevel)
        {
            _claim = AuthenticationBuilder.CreateClaim(authenticationLevel);
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            _dataBuilder = Container.GetService<ChapterDataBuilder>();
            var chapters = _dataBuilder.WithLibrary(LibraryId).WithContents().Build(4);
            _book = DatabaseConnection.GetBookById(chapters.PickRandom().BookId);

            _response = (OkObjectResult)await handler.Run(LibraryId, _book.Id, _claim, CancellationToken.None);

            _view = _response.Value as ListView<ChapterView>;
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldReturnOk()
        {
            _response.ShouldBeOk();
        }

        [Test]
        public void ShouldHaveSelfLink()
        {
            _view.SelfLink()
                .ShouldBeGet()
                .EndingWith($"library/{LibraryId}/books/{_book.Id}/chapters");
        }

        [Test]
        public void ShouldHaveCreateLink()
        {
            _view.CreateLink()
                .ShouldBePost()
                .EndingWith($"library/{LibraryId}/books/{_book.Id}/chapters");
        }

        [Test]
        public void ShouldHaveCorrectNumberOfChapters()
        {
            Assert.That(_view.Data.Count(), Is.EqualTo(4));
        }

        [Test]
        public void ShouldHaveCorrectChaptersData()
        {
            foreach (var expected in _dataBuilder.Chapters.Where(c => c.BookId == _book.Id))
            {
                var actual = _view.Data.FirstOrDefault(x => x.Id == expected.Id);
                var assert = new ChapterAssert(actual, LibraryId);
                assert.ShouldBeSameAs(expected)
                      .WithWriteableLinks();

                var contents = _dataBuilder.Contents.Where(c => c.ChapterId == expected.Id);
                foreach (var content in contents)
                {
                    assert.ShouldHaveContentLink(content);
                }

                assert.ShouldHaveUpdateChapterContentLink();
                assert.ShouldHaveDeleteChapterContentLink();
            }
        }
    }
}
