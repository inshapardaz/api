using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Bogus;
using Inshapardaz.Functions.Tests.DataBuilders;
using Inshapardaz.Functions.Tests.Dto;
using Inshapardaz.Functions.Tests.Helpers;
using Inshapardaz.Functions.Views;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Functions.Tests.Library.Book.Files.UpdateBookFile
{
    [TestFixture, Ignore("ToFix")]
    public class WhenUpdatingBookFileForForFileNotExisting : LibraryTest<Functions.Library.Books.Files.UpdateBookFile>
    {
        private CreatedResult _response;

        private BookDto _book;
        private FileView _view;
        private byte[] _expected;
        private BooksDataBuilder _dataBuilder;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _dataBuilder = Container.GetService<BooksDataBuilder>();

            _book = _dataBuilder.Build();
            _expected = new Faker().Image.Random.Bytes(50);
            var request = new RequestBuilder().WithBytes(_expected).BuildRequestMessage();
            _response = (CreatedResult)await handler.Run(request, LibraryId, _book.Id, Random.Number, AuthenticationBuilder.AdminClaim, CancellationToken.None);

            _view = (FileView)_response.Value;
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldHaveCreatedResult()
        {
            Assert.That(_response, Is.Not.Null);
            Assert.That(_response.StatusCode, Is.EqualTo((int)HttpStatusCode.Created));
        }

        [Test]
        public void ShouldLocationHeader()
        {
            Assert.That(_response.Location, Is.Not.Empty);
        }

        [Test, Ignore("Need attention")]
        public async Task ShouldHaveUpdatedFileContents()
        {
            await Check.ThatFileContentsMatch(_view.Id, _expected);
        }
    }
}
