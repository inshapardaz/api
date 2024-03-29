﻿using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.Dto;
using Inshapardaz.Domain.Models;
using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.Chapter.DeleteChapter
{
    [TestFixture(Role.Admin)]
    [TestFixture(Role.LibraryAdmin)]
    [TestFixture(Role.Writer)]
    public class WhenDeletingChapterWithPermission
        : TestBase
    {
        private HttpResponseMessage _response;
        private ChapterDto _expected;

        public WhenDeletingChapterWithPermission(Role role)
            : base(role)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            _expected = ChapterBuilder.WithLibrary(LibraryId).WithContents().WithPages().Build();

            _response = await Client.DeleteAsync($"/libraries/{LibraryId}/books/{_expected.BookId}/chapters/{_expected.ChapterNumber}");
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldReturnNoContent()
        {
            _response.ShouldBeNoContent();
        }

        [Test]
        public void ShouldHaveDeletedChapter()
        {
            ChapterAssert.ShouldHaveDeletedChapter(_expected.Id, DatabaseConnection);
        }

        [Test]
        public void ShouldHaveDeletedTheChapterContents()
        {
            ChapterAssert.ThatContentsAreDeletedForChapter(_expected.Id, DatabaseConnection);
        }
    }
}
