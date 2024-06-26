﻿using FluentAssertions;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.BookPage.UpdatePageSequenceNumber
{
    [TestFixture(Role.Admin)]
    [TestFixture(Role.LibraryAdmin)]
    [TestFixture(Role.Writer)]
    public class WhenUpdatingPageSequenceNumber : TestBase
    {
        private HttpResponseMessage _response;
        private BookPageAssert _assert;
        private BookPageDto _page;
        private int _bookId, _newSequenceNumber;
        private string _text;

        public WhenUpdatingPageSequenceNumber(Role role)
            : base(role)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var book = BookBuilder.WithLibrary(LibraryId).WithPages(5, true).Build();
            _page = BookBuilder.GetPages(book.Id).ElementAt(2);

            _text = RandomData.Text;

            var oldSequenceNumber = _page.SequenceNumber;
            _newSequenceNumber = 1;
            _page.SequenceNumber = _newSequenceNumber;

            _bookId = book.Id;
            _response = await Client.PostObject($"/libraries/{LibraryId}/books/{_bookId}/pages/{oldSequenceNumber}/sequenceNumber", new { SequenceNumber = _page.SequenceNumber });
            _assert = Services.GetService<BookPageAssert>().ForResponse(_response).ForLibrary(LibraryId);
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
        public void ShouldHaveUpdatedThePageSequenceNumber()
        {
            var savedPage = BookPageTestRepository.GetBookPageById(_page.BookId, _page.Id);
            savedPage.Should().NotBeNull();
            savedPage.SequenceNumber.Should().Be(1);
        }
    }
}
