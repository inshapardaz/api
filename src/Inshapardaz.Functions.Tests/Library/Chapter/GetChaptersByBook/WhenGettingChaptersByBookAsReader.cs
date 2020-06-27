using System.Linq;
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
    [TestFixture]
    public class WhenGettingChaptersByBookAsReader : LibraryTest<Functions.Library.Books.Chapters.GetChaptersByBook>
    {
        private BookDto _book;
        private OkObjectResult _response;
        private ListView<ChapterView> _view;
        private ChapterDataBuilder _dataBuilder;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var request = new RequestBuilder().Build();
            _dataBuilder = Container.GetService<ChapterDataBuilder>();
            var chapters = _dataBuilder.WithLibrary(LibraryId).WithContents().Build(4);
            _book = DatabaseConnection.GetBookById(chapters.PickRandom().BookId);
            _response = (OkObjectResult)await handler.Run(request, LibraryId, _book.Id, AuthenticationBuilder.ReaderClaim, CancellationToken.None);

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
        public void ShouldNotHaveCreateLink()
        {
            _view.CreateLink().Should().BeNull();
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
                      .WithReadOnlyLinks();

                var contents = _dataBuilder.Contents.Where(c => c.ChapterId == expected.Id);
                foreach (var content in contents)
                {
                    assert.ShouldHaveContentLink(content);
                }
            }
        }
    }
}
