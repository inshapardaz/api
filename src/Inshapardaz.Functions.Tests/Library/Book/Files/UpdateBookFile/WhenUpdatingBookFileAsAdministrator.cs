using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Bogus;
using Inshapardaz.Domain.Repositories;
using Inshapardaz.Functions.Tests.DataBuilders;
using Inshapardaz.Functions.Tests.Fakes;
using Inshapardaz.Functions.Tests.Helpers;
using Inshapardaz.Functions.Views;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Functions.Tests.Library.Book.Files.UpdateBookFile
{
    [TestFixture]
    public class WhenUpdatingBookFileAsAdministrator : FunctionTest
    {
        private OkObjectResult _response;

        private Ports.Database.Entities.Library.Book _book;
        private FileView _view;
        private byte[] _expected;
        private BooksDataBuilder _dataBuilder;
        private FakeFileStorage _fileStorage;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _dataBuilder = Container.GetService<BooksDataBuilder>();
            _fileStorage = Container.GetService<IFileStorage>() as FakeFileStorage;

            _book = _dataBuilder.WithFile().Build();
            _expected = new Faker().Image.Random.Bytes(50);
            var request = new RequestBuilder().WithBytes(_expected).BuildRequestMessage();
            var handler = Container.GetService<Functions.Library.Books.Files.UpdateBookFile>();
            _response = (OkObjectResult)await handler.Run(request, _book.Id, _book.Files.First().Id, AuthenticationBuilder.AdminClaim, CancellationToken.None);

            _view = (FileView)_response.Value;
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldReturnOk()
        {
            Assert.That(_response, Is.Not.Null);
            Assert.That(_response.StatusCode, Is.EqualTo(200));
        }

        [Test, Ignore("Need attention")]
        public async Task ShouldHaveUpdatedFileContents()
        {
            var file = _dataBuilder.GetFileById(_view.Id);
            var contents = await _fileStorage.GetFile(file.FilePath, CancellationToken.None);
            Assert.That(contents, Is.EqualTo(_expected), "File contents should have updated.");
        }
    }
}
