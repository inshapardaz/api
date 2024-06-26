using FluentAssertions;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Api.Views;
using Inshapardaz.Api.Views.Library;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.Chapter.GetChaptersByBook
{
    [TestFixture]
    public class WhenGettingChaptersByBookAsUnauthorized
        : TestBase
    {
        private HttpResponseMessage _response;
        private ListView<ChapterView> _view;
        private IEnumerable<ChapterDto> _chapters;
        private BookDto _book;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _chapters = ChapterBuilder.WithLibrary(LibraryId).WithContents().Build(4);
            _book = BookTestRepository.GetBookById(_chapters.PickRandom().BookId);

            _response = await Client.GetAsync($"/libraries/{LibraryId}/books/{_book.Id}/chapters");
            _view = _response.GetContent<ListView<ChapterView>>().Result;
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
                .EndingWith($"libraries/{LibraryId}/books/{_book.Id}/chapters");
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
            foreach (var item in _chapters.Where(c => c.BookId == _book.Id))
            {
                var actual = _view.Data.FirstOrDefault(x => x.Id == item.Id);
                Services.GetService<ChapterAssert>().ForView(actual).ForLibrary(LibraryId)
                      .WithReadOnlyLinks()
                      .ShouldNotHaveContentsLink();
            }
        }
    }
}
