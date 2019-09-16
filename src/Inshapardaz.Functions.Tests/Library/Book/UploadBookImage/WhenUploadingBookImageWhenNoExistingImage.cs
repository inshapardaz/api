﻿using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Repositories;
using Inshapardaz.Functions.Tests.DataBuilders;
using Inshapardaz.Functions.Tests.Fakes;
using Inshapardaz.Functions.Tests.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Functions.Tests.Library.Book.UploadBookImage
{
    [TestFixture]
    public class WhenUploadingBookImageWhenNoExistingImage : FunctionTest
    {
        private CreatedResult _response;
        private BooksDataBuilder _builder;
        private FakeFileStorage _fileStorage;
        private int _bookId;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _builder = Container.GetService<BooksDataBuilder>();
            _fileStorage = Container.GetService<IFileStorage>() as FakeFileStorage;
            
            var book = _builder.WithNoImage().Build();
            _bookId = book.Id;
            var handler = Container.GetService<Functions.Library.Books.UpdateBookImage>();
            var request = new RequestBuilder().WithImage().BuildRequestMessage();
            _response = (CreatedResult) await handler.Run(request, _bookId, AuthenticationBuilder.WriterClaim, CancellationToken.None);
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
        public async Task ShouldHaveUpdatedBookImage()
        {
            var imageUrl = _builder.GetBookImageUrl(_bookId);
            Assert.That(imageUrl, Is.Not.Null, "Book should have an image url`.");
            var image = await _fileStorage.GetFile(imageUrl, CancellationToken.None);
            Assert.That(image, Is.Not.Null, "Book should have an image.");
        }
    }
}