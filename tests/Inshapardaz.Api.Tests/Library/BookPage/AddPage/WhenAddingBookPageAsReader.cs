﻿using Bogus;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Models;
using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.BookPage.AddPage
{
    [TestFixture]
    public class WhenAddingBookPageAsReader
        : TestBase
    {
        private HttpResponseMessage _response;

        public WhenAddingBookPageAsReader()
            : base(Role.Reader)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var book = BookBuilder.WithLibrary(LibraryId).Build();

            var page = new BookPageView { Text = new Faker().Random.String(), SequenceNumber = 1 };
            _response = await Client.PostObject($"/libraries/{LibraryId}/books/{book.Id}/pages", page);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldHaveForbiddenResult()
        {
            _response.ShouldBeForbidden();
        }
    }
}