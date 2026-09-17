using FluentAssertions;
using Inshapardaz.Api.Tests.Framework.DataHelpers;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Api.Views.Library;
using DateTime = System.DateTime;

namespace Inshapardaz.Api.Tests.Framework.Asserts
{
    public class NoteAssert(IBookTestRepository bookTestRepository)
    {
        private NoteView _view;
        private IEnumerable<NoteView> _views;

        public NoteAssert ForResponse(HttpResponseMessage response)
        {
            _view = response.GetContent<NoteView>().Result;
            return this;
        }

        public NoteAssert ForListResponse(HttpResponseMessage response)
        {
            _views = response.GetContent<IEnumerable<NoteView>>().Result;
            return this;
        }

        public NoteAssert ShouldMatch(NoteView expected)
        {
            _view.Should().NotBeNull();
            _view.Id.Should().Be(expected.Id);
            _view.ChapterId.Should().Be(expected.ChapterId);
            _view.StartOffset.Should().Be(expected.StartOffset);
            _view.EndOffset.Should().Be(expected.EndOffset);
            _view.Text.Should().Be(expected.Text);
            _view.Comment.Should().Be(expected.Comment);
            return this;
        }

        public NoteAssert ShouldContain(NoteView expected)
        {
            _views.Should().Contain(n => n.Id == expected.Id
                && n.ChapterId == expected.ChapterId
                && n.StartOffset == expected.StartOffset
                && n.EndOffset == expected.EndOffset
                && n.Text == expected.Text
                && n.Comment == expected.Comment);
            return this;
        }

        public NoteAssert ShouldHaveCount(int count)
        {
            _views.Should().HaveCount(count);
            return this;
        }

        public NoteAssert ShouldHaveSaved(int bookId, int accountId, string clientId, NoteView expected)
        {
            var note = bookTestRepository.GetNote(bookId, accountId, clientId);

            note.Should().NotBeNull();
            note.ChapterId.Should().Be(expected.ChapterId);
            note.StartOffset.Should().Be(expected.StartOffset);
            note.EndOffset.Should().Be(expected.EndOffset);
            note.Text.Should().Be(expected.Text);
            note.Comment.Should().Be(expected.Comment);

            return this;
        }

        public NoteAssert ShouldHaveBeenAdded(int bookId, int accountId, string clientId)
        {
            var note = bookTestRepository.GetNote(bookId, accountId, clientId);

            note.Should().NotBeNull();
            note.DateAdded.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
            note.DateUpdated.Should().BeNull();

            return this;
        }

        public NoteAssert ShouldHaveBeenUpdated(int bookId, int accountId, string clientId)
        {
            var note = bookTestRepository.GetNote(bookId, accountId, clientId);

            note.Should().NotBeNull();
            note.DateUpdated.Should().NotBeNull();
            note.DateUpdated.Value.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));

            return this;
        }

        public NoteAssert ShouldNotExist(int bookId, int accountId, string clientId)
        {
            bookTestRepository.GetNote(bookId, accountId, clientId).Should().BeNull();
            return this;
        }
    }
}
