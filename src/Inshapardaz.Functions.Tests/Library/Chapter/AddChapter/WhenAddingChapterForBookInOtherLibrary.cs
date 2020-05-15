using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Functions.Tests.Asserts;
using Inshapardaz.Functions.Tests.DataBuilders;
using Inshapardaz.Functions.Tests.Helpers;
using Inshapardaz.Functions.Views.Library;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Functions.Tests.Library.Chapter.AddChapter
{
    [TestFixture]
    public class WhenAddingChapterForBookInOtherLibrary
        : LibraryTest<Functions.Library.Books.Chapters.AddChapter>
    {
        private BadRequestResult _response;
        private LibraryDataBuilder _builder;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _builder = Container.GetService<LibraryDataBuilder>();
            var library2 = _builder.Build();
            var book = Container.GetService<BooksDataBuilder>().WithLibrary(library2.Id).Build();
            var chapter = new ChapterView { Title = Random.Name, ChapterNumber = 1, BookId = book.Id };

            _response = (BadRequestResult)await handler.Run(chapter, LibraryId, book.Id, AuthenticationBuilder.WriterClaim, CancellationToken.None);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            _builder.CleanUp();
            Cleanup();
        }

        [Test]
        public void ShouldHaveBadRequestResult()
        {
            _response.ShouldBeBadRequest();
        }
    }
}
