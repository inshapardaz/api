using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.DataBuilders;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.BookPage.AddPage
{
    [TestFixture]
    public class WhenAddingBookPageForBookInOtherLibrary() : TestBase(Role.Writer)
    {
        private HttpResponseMessage _response;
        private LibraryDataBuilder _libBuilder;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _libBuilder = Services.GetService<LibraryDataBuilder>();

            var library2 = _libBuilder.Build();
            var book = BookBuilder.WithLibrary(library2.Id).Build();

            var page = new BookPageView { Text = RandomData.Text, SequenceNumber = 1 };
            _response = await Client.PostObject($"/libraries/{LibraryId}/books/{book.Id}/pages", page);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            _libBuilder.CleanUp();
            Cleanup();
        }

        [Test]
        public void ShouldHaveBadRequestResult() => _response.ShouldBeBadRequest();
    }
}
