using FluentAssertions;
using Inshapardaz.Api.Tests.Framework.DataHelpers;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Api.Views.Library;
using DateTime = System.DateTime;

namespace Inshapardaz.Api.Tests.Framework.Asserts
{
    public class BookmarkAssert(IBookTestRepository bookTestRepository)
    {
        private BookmarkView _view;
        private IEnumerable<BookmarkView> _views;

        public BookmarkAssert ForResponse(HttpResponseMessage response)
        {
            _view = response.GetContent<BookmarkView>().Result;
            return this;
        }

        public BookmarkAssert ForListResponse(HttpResponseMessage response)
        {
            _views = response.GetContent<IEnumerable<BookmarkView>>().Result;
            return this;
        }

        public BookmarkAssert ShouldMatch(BookmarkView expected)
        {
            _view.Should().NotBeNull();
            _view.Id.Should().Be(expected.Id);
            _view.ChapterId.Should().Be(expected.ChapterId);
            _view.Position.Should().Be(expected.Position);
            _view.Name.Should().Be(expected.Name);
            return this;
        }

        public BookmarkAssert ShouldContain(BookmarkView expected)
        {
            _views.Should().Contain(b => b.Id == expected.Id
                && b.ChapterId == expected.ChapterId
                && b.Position == expected.Position
                && b.Name == expected.Name);
            return this;
        }

        public BookmarkAssert ShouldHaveCount(int count)
        {
            _views.Should().HaveCount(count);
            return this;
        }

        public BookmarkAssert ShouldHaveSaved(int bookId, int accountId, string clientId, BookmarkView expected)
        {
            var bookmark = bookTestRepository.GetBookmark(bookId, accountId, clientId);

            bookmark.Should().NotBeNull();
            bookmark.ChapterId.Should().Be(expected.ChapterId);
            bookmark.Position.Should().Be(expected.Position);
            bookmark.Name.Should().Be(expected.Name);

            return this;
        }

        public BookmarkAssert ShouldHaveBeenAdded(int bookId, int accountId, string clientId)
        {
            var bookmark = bookTestRepository.GetBookmark(bookId, accountId, clientId);

            bookmark.Should().NotBeNull();
            bookmark.DateAdded.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
            bookmark.DateUpdated.Should().BeNull();

            return this;
        }

        public BookmarkAssert ShouldHaveBeenUpdated(int bookId, int accountId, string clientId)
        {
            var bookmark = bookTestRepository.GetBookmark(bookId, accountId, clientId);

            bookmark.Should().NotBeNull();
            bookmark.DateUpdated.Should().NotBeNull();
            bookmark.DateUpdated.Value.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));

            return this;
        }

        public BookmarkAssert ShouldNotExist(int bookId, int accountId, string clientId)
        {
            bookTestRepository.GetBookmark(bookId, accountId, clientId).Should().BeNull();
            return this;
        }
    }
}
