using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Inshapardaz.Functions.Tests.Asserts;
using Inshapardaz.Functions.Tests.DataBuilders;
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
    public class WhenGettingChaptersByBookWithNoChapters
        : LibraryTest<Functions.Library.Books.Chapters.GetChaptersByBook>
    {
        private BookDto _book;
        private OkObjectResult _response;
        private ListView<ChapterView> _view;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var request = new RequestBuilder().Build();
            var dataBuilder = Container.GetService<BooksDataBuilder>();
            _book = dataBuilder.WithLibrary(LibraryId).Build();

            _response = (OkObjectResult)await handler.Run(request, LibraryId, _book.Id, AuthenticationBuilder.WriterClaim, CancellationToken.None);

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
            _view.CreateLink()
                .ShouldBePost()
                .EndingWith($"library/{LibraryId}/books/{_book.Id}/chapters");
        }

        [Test]
        public void ShouldHaveNoChapters()
        {
            _view.Data.Should().BeEmpty();
        }
    }
}
