using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;

namespace Inshapardaz.Domain.Adapters.Repositories.Library;

public interface IBookRepository
{
    Task<BookModel> AddBook(int libraryId, BookModel book, int? accountId, CancellationToken cancellationToken);

    Task UpdateBook(int libraryId, BookModel book, CancellationToken cancellationToken);

    Task DeleteBook(int libraryId, int bookId, CancellationToken cancellationToken);

    Task<Page<BookModel>> GetBooks(int libraryId, int pageNumber, int pageSize, int? accountId, BookFilter filter, BookSortByType sortBy, SortDirection direction, CancellationToken cancellationToken);

    Task<Page<BookModel>> SearchBooks(int libraryId, string searchText, int pageNumber, int pageSize, int? accountId, BookFilter filter, BookSortByType sortBy, SortDirection direction, CancellationToken cancellationToken);

    Task<BookModel> GetBookById(int libraryId, long bookId, int? accountId, CancellationToken cancellationToken);

    Task<BookModel> GetBookBySource(int libraryId, string source, CancellationToken cancellationToken);

    Task<ReadProgressModel> AddRecentBook(int libraryId, int accountId, int bookId, ReadProgressModel progress,
        CancellationToken cancellationToken);

    Task DeleteBookFromRecent(int libraryId, int accountId, int bookId, CancellationToken cancellationToken);

    Task AddBookToFavorites(int libraryId, int? accountId, int bookId, CancellationToken cancellationToken);
    Task<Page<BookModel>> GetBooksByUser(int libraryId, int accountId, int pageNumber, int pageSize, StatusType status, BookSortByType sortBy, SortDirection direction, CancellationToken cancellationToken);
    Task DeleteBookFromFavorites(int libraryId, int accountId, int bookId, CancellationToken cancellationToken);

    Task<int> AddBookContent(int bookId, long fileId, string language, CancellationToken cancellationToken);

    Task DeleteBookContent(int libraryId, int bookId, long contentId, CancellationToken cancellationToken);

    Task<BookContentModel> GetBookContent(int libraryId, int bookId, string language, string mimeType, CancellationToken cancellationToken);
    Task<BookContentModel> GetBookContent(int libraryId, int bookId, long conetntId, CancellationToken cancellationToken);

    Task<IEnumerable<BookContentModel>> GetBookContents(int libraryId, int bookId, CancellationToken cancellationToken);

    Task UpdateBookImage(int libraryId, int bookId, long fileId, CancellationToken cancellationToken);

    Task UpdateBookContent(int libraryId, int bookId, int contentId, string language, CancellationToken cancellationToken);

    Task<IEnumerable<PageSummaryModel>> GetBookPageSummary(int libraryId, IEnumerable<int> bookIds, CancellationToken cancellationToken);

    #region for migration
    Task<Page<BookModel>> GetBooks(int libraryId, int pageNumber, int pageSize, CancellationToken cancellationToken);
    #endregion

    Task<Page<string>> FindPublishers(int libraryId, string query, int pageNumber, int pageSize,
        CancellationToken cancellationToken);

    Task<IEnumerable<BookmarkModel>> GetBookmarks(int libraryId, int accountId, int bookId, CancellationToken cancellationToken);

    Task<BookmarkModel> UpsertBookmark(int libraryId, int accountId, int bookId, string clientId, BookmarkModel bookmark, CancellationToken cancellationToken);

    Task DeleteBookmark(int libraryId, int accountId, int bookId, string clientId, CancellationToken cancellationToken);

    Task<IEnumerable<NoteModel>> GetNotes(int libraryId, int accountId, int bookId, CancellationToken cancellationToken);

    Task<NoteModel> UpsertNote(int libraryId, int accountId, int bookId, string clientId, NoteModel note, CancellationToken cancellationToken);

    Task DeleteNote(int libraryId, int accountId, int bookId, string clientId, CancellationToken cancellationToken);

    Task<RatingModel> GetBookRating(int libraryId, int accountId, int bookId, CancellationToken cancellationToken);

    Task<RatingModel> UpsertBookRating(int libraryId, int accountId, int bookId, RatingModel rating, CancellationToken cancellationToken);

    Task DeleteBookRating(int libraryId, int accountId, int bookId, CancellationToken cancellationToken);

    Task<RatingSummaryModel> GetBookRatingSummary(int libraryId, int bookId, CancellationToken cancellationToken);

    Task<IEnumerable<CategoryModel>> GetBookCategories(int libraryId, int bookId, CancellationToken cancellationToken);

    Task SetBookCategories(int libraryId, int bookId, IEnumerable<int> categoryIds, CancellationToken cancellationToken);

    Task AddCategoryToBook(int libraryId, int bookId, int categoryId, CancellationToken cancellationToken);

    Task RemoveCategoryFromBook(int libraryId, int bookId, int categoryId, CancellationToken cancellationToken);
}
