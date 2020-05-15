using System.Threading;
using System.Threading.Tasks;
using Bogus;
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
    public class WhenAddingChapterAsUnauthorizedUser
        : LibraryTest<Functions.Library.Books.Chapters.AddChapter>
    {
        private UnauthorizedResult _response;
        private BooksDataBuilder _builder;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _builder = Container.GetService<BooksDataBuilder>();

            var book = _builder.WithLibrary(LibraryId).Build();

            var chapter = new ChapterView { Title = new Faker().Random.String(), ChapterNumber = 1 };

            _response = (UnauthorizedResult)await handler.Run(chapter, LibraryId, book.Id, AuthenticationBuilder.Unauthorized, CancellationToken.None);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldHaveUnauthorisedResult()
        {
            _response.ShouldBeUnauthorized();
        }
    }
}
