using FluentAssertions;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.DataHelpers;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Api.Views;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Models;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.Chapter.UpdateChapterSequence
{
    [TestFixture]
    public class WhenUpdatingChapterSequence
        : TestBase
    {
        private HttpResponseMessage _response;
        private ListView<ChapterView> _view;
        private IEnumerable<ChapterDto> _chapters;
        private int _bookId;

        public WhenUpdatingChapterSequence()
            : base(Role.Writer)
        {

        }

        [OneTimeSetUp]
        public async Task Setup()
        {

            _chapters = ChapterBuilder.WithLibrary(LibraryId).Public().Build(3);
            _bookId = _chapters.First().BookId;

            var payload = _chapters.Reverse();
            int i = 1;
            foreach (var p in payload)
            {
                p.ChapterNumber = i++;
            }

            _response = await Client.PostObject($"/libraries/{LibraryId}/books/{_bookId}/chapters/sequence", payload);

            _view = _response.GetContent<ListView<ChapterView>>().Result;
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldHaveOkResult()
        {
            _response.ShouldBeOk();
        }

        [Test]
        public void ShouldHaveChangedTheOrderOfChapters()
        {
            var expectedChapters = _chapters.Reverse();
            var actualChapters = DatabaseConnection.GetChaptersByBook(_bookId);
            int i = 1;
            foreach (var expected in expectedChapters)
            {
                expected.ChapterNumber = i++;

                var actual = actualChapters.FirstOrDefault(c => c.Id == expected.Id);

                actual.Should().NotBeNull();
                actual.ChapterNumber.Should().Be(expected.ChapterNumber);
            }

        }
    }
}
