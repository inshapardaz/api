using Dapper;
using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Database.SqlServer.Repositories.Library
{
    public class BookPageRepository : IBookPageRepository
    {
        private readonly IProvideConnection _connectionProvider;

        public BookPageRepository(IProvideConnection connectionProvider)
        {
            _connectionProvider = connectionProvider;
        }

        public async Task<BookPageModel> AddPage(int libraryId, int bookId, int sequenceNumber, string text, int imageId, CancellationToken cancellationToken)
        {
            int authorId;
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"Insert Into BookPage(BookId, SequenceNumber, Text, ImageId) OUTPUT Inserted.Id VALUES(@BookId, @SequenceNumber, @Text, @ImageId);";
                var command = new CommandDefinition(sql, new { BookId = bookId, SequenceNumber = sequenceNumber, Text = text, ImageId = imageId }, cancellationToken: cancellationToken);
                authorId = await connection.ExecuteScalarAsync<int>(command);
            }

            return await GetPageBySequenceNumber(libraryId, bookId, sequenceNumber, cancellationToken);
        }

        public async Task<BookPageModel> GetPageBySequenceNumber(int libraryId, int bookId, int sequenceNumber, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"SELECT p.BookId, p.SequenceNumber, p.Text, f.Id As ImageId
                            FROM BookPage AS p
                            LEFT OUTER JOIN [File] f ON f.Id = p.ImageId
                            Where p.BookId = @BookId AND p.SequenceNumber = @SequenceNumber";
                var command = new CommandDefinition(sql,
                                                    new { BookId = bookId, SequenceNumber = sequenceNumber },
                                                    cancellationToken: cancellationToken);

                return await connection.QuerySingleOrDefaultAsync<BookPageModel>(command);
            }
        }

        public async Task DeletePage(int libraryId, int bookId, int sequenceNumber, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"Delete From BookPage Where BookId = @BookId AND SequenceNumber = @SequenceNumber";
                var command = new CommandDefinition(sql, new { BookId = bookId, SequenceNumber = sequenceNumber }, cancellationToken: cancellationToken);
                await connection.ExecuteAsync(command);
            }
        }

        public async Task<BookPageModel> UpdatePage(int libraryId, int bookId, int sequenceNumber, string text, int imageId, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"Update BookPage Set Text = @Text, ImageId = @ImageId Where BookId = @BookId AND SequenceNumber = @SequenceNumber";
                var command = new CommandDefinition(sql, new { Text = text, ImageId = imageId, BookId = bookId, SequenceNumber = sequenceNumber }, cancellationToken: cancellationToken);
                await connection.ExecuteAsync(command);

                return await GetPageBySequenceNumber(libraryId, bookId, sequenceNumber, cancellationToken);
            }
        }

        public async Task<BookPageModel> UpdatePageImage(int libraryId, int bookId, int sequenceNumber, int imageId, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"Update BookPage Set ImageId = @ImageId Where BookId = @BookId AND SequenceNumber = @SequenceNumber";
                var command = new CommandDefinition(sql, new { ImageId = imageId, BookId = bookId, SequenceNumber = sequenceNumber }, cancellationToken: cancellationToken);
                await connection.ExecuteAsync(command);

                return await GetPageBySequenceNumber(libraryId, bookId, sequenceNumber, cancellationToken);
            }
        }

        public async Task DeletePageImage(int libraryId, int bookId, int sequenceNumber, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"Update BookPage Set ImageId = NULL Where BookId = @BookId AND SequenceNumber = @SequenceNumber";
                var command = new CommandDefinition(sql, new { BookId = bookId, SequenceNumber = sequenceNumber }, cancellationToken: cancellationToken);
                await connection.ExecuteAsync(command);
            }
        }

        public async Task<Page<BookPageModel>> GetPagesByBook(int libraryId, int bookId, int pageNumber, int pageSize, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"SELECT p.BookId, p.SequenceNumber, p.Text, f.Id As ImageId
                            FROM BookPage AS p
                            LEFT OUTER JOIN [File] f ON f.Id = p.ImageId
                            WHERE p.BookId = @BookId
                            ORDER BY p.SequenceNumber
                            OFFSET @PageSize * (@PageNumber - 1) ROWS
                            FETCH NEXT @PageSize ROWS ONLY";
                var command = new CommandDefinition(sql,
                                                    new { BookId = bookId, PageSize = pageSize, PageNumber = pageNumber },
                                                    cancellationToken: cancellationToken);

                var pages = await connection.QueryAsync<BookPageModel>(command);

                var sqlCount = @"SELECT Count(*) FROM BookPage Where BookId = @BookId";
                var commandCount = new CommandDefinition(sqlCount, new { BookId = bookId },
                                                    cancellationToken: cancellationToken);

                var pagesCount = await connection.QuerySingleAsync<int>(commandCount);
                return new Page<BookPageModel>
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalCount = pagesCount,
                    Data = pages
                };
            }
        }
    }
}
