using System.Threading;
using System.Threading.Tasks;
using Bogus;
using Inshapardaz.Functions.Tests.DataBuilders;
using Inshapardaz.Functions.Tests.Helpers;
using Inshapardaz.Functions.Views.Library;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Functions.Tests.Library.Chapter.AddChapter
{
    [TestFixture]
    public class WhenAddingChapterForNonExistingBook : FunctionTest
    {
        private BadRequestResult _response;
        private BooksDataBuilder _builder;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _builder = Container.GetService<BooksDataBuilder>();

            var handler = Container.GetService<Functions.Library.Books.Chapters.AddChapter>();
            var chapter = new ChapterView { Title = new Faker().Random.String(), ChapterNumber = 1 };
            var request = new RequestBuilder()
                                            .WithJsonBody(chapter)
                                            .Build();
            _response = (BadRequestResult)await handler.Run(request, new Faker().Random.Int(), AuthenticationBuilder.WriterClaim, CancellationToken.None);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldHaveBadRequestResult()
        {
            Assert.That(_response, Is.Not.Null);
        }
    }
}
