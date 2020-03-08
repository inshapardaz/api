﻿using System.Threading;
using System.Threading.Tasks;
using Bogus;
using Inshapardaz.Functions.Tests.DataBuilders;
using Inshapardaz.Functions.Tests.Helpers;
using Inshapardaz.Functions.Views;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Functions.Tests.Library.Book.Files.AddBookFile
{
    [TestFixture]
    public class WhenAddingBookFileAsAdministrator : LibraryTest<Functions.Library.Books.Files.AddBookFile>
    {
        private CreatedResult _response;
        private BooksDataBuilder _dataBuilder;
        private FileView _view;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _dataBuilder = Container.GetService<BooksDataBuilder>();

            var book = _dataBuilder.Build();
            var contents = new Faker().Random.Words(60);
            var request = new RequestBuilder().WithBody(contents).BuildRequestMessage();
            _response = (CreatedResult)await handler.Run(request, LibraryId, book.Id, AuthenticationBuilder.AdminClaim, CancellationToken.None);

            _view = _response.Value as FileView;
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
            Assert.That(_response.StatusCode, Is.EqualTo(201));
        }

        [Test]
        public void ShouldHaveSelfLink()
        {
            _view.Links.AssertLink("self")
                 .ShouldBeGet()
                 .ShouldHaveSomeHref();
        }

        [Test, Ignore("have not implemented yet")]
        public void ShouldHaveBookLink()
        {
            _view.Links.AssertLink("book")
                 .ShouldBeGet()
                 .ShouldHaveSomeHref();
        }

        [Test, Ignore("have not implemented yet")]
        public void ShouldHaveUpdateLink()
        {
            _view.Links.AssertLink("update")
                 .ShouldBePut()
                 .ShouldHaveSomeHref();
        }

        [Test]
        public void ShouldHaveDeleteLink()
        {
            _view.Links.AssertLink("delete")
                 .ShouldBeDelete()
                 .ShouldHaveSomeHref();
        }

        [Test, Ignore("Need attention")]
        public void ShouldReturnCorrectBookFile()
        {
            Check.ThatFileExists(_view.Id);
        }
    }
}
