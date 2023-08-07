using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;

namespace Inshapardaz.Database.SqlServer.Repositories.Library
{
    public class ArticleRepository : IArticleRepository
    {
        private readonly IProvideConnection _connectionProvider;

        public ArticleRepository(IProvideConnection connectionProvider)
        {
            _connectionProvider = connectionProvider;
        }
        public async Task<ArticleModel> AddArticle(int libraryId, ArticleModel article, CancellationToken cancellationToken)
        {
            int id;
            using (var connection = _connectionProvider.GetLibraryConnection())
            {
                var sql = @"INSERT INTO Article (Title, Status, WriterAccountId, WriterAssignTimestamp, ReviewerAccountId, ReviewerAssignTimeStamp) 
                            OUTPUT Inserted.Id VALUES (@Title, @Status, @WriterAccountId, @WriteAssignTimestamp, @ReviewerAccountId, @ReviewerAssignTimeStamp)";
                var command = new CommandDefinition(sql, new
                {
                    Title = article.Title,
                    Status = article.Status,
                    WriterAccountId = article.WriterAccountId,
                    WriteAssignTimestamp = article.WriterAssignTimeStamp,
                    ReviewerAccountId = article.ReviewerAccountId,
                    ReviewerAssignTimeStamp = article.ReviewerAssignTimeStamp
                }, cancellationToken: cancellationToken);
                id = await connection.ExecuteScalarAsync<int>(command);

                var sqlAuthor = @"Insert Into ArticleAuthor (ArticleId, AuthorId) Values (@ArticleId, @AuthorId);";

                if (article.Authors != null && article.Authors.Any())
                {
                    var bookAuthors = article.Authors.Select(a => new { ArticleId = article.Id, AuthorId = a.Id });
                    var commandCategory = new CommandDefinition(sqlAuthor, bookAuthors, cancellationToken: cancellationToken);
                    await connection.ExecuteAsync(commandCategory);
                }
            }

            return await GetArticleById(id, cancellationToken);
        }

        public Task<ArticleContentModel> AddArticleContent(int libraryId, long articleId, string language, string content, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public Task DeleteArticle(int libraryId, long articleId, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public Task DeleteArticleContent(int libraryId, long articleId, string language, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public Task<ArticleModel> GetArticle(int libraryId, long articleId, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public Task<ArticleContentModel> GetArticleContent(int libraryId, long articleId, string language, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public Task<ArticleContentModel> GetArticleContentById(int libraryId, long articleId, string language, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public Task<IEnumerable<ArticleContentModel>> GetArticleContents(int libraryId, long articleId, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public Task<Page<ArticleModel>> GetArticles(int libraryId, int pageNumber, int pageSize, int? accountId, ArticleFilter filter, ArticleSortByType sortBy, SortDirection sortDirection, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public Task<Page<ArticleModel>> SearchArticles(int libraryId, string query, int pageNumber, int pageSize, int? accountId, ArticleFilter filter, ArticleSortByType sortBy, SortDirection sortDirection, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public Task UpdateArticle(int libraryId, long articleId, ArticleModel article, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public Task<ArticleContentModel> UpdateArticleContent(int libraryId, long articleId, string language, string content, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public Task<ArticleModel> UpdateReviewerAssignment(int libraryId, long articleId, int? accountId, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public Task<ArticleModel> UpdateWriterAssignment(int libraryId, long articleId, int? accountId, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        private async Task<ArticleModel> GetArticleById(int articleId, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetLibraryConnection())
            {
                ArticleModel article = null;
                var sql = @"SELECT a.*, aa.*, ac.*
                            FROM Article a
                            INNER JOIN ArticleAuthor aa ON aa.ArticleId = a.Id
                            INNER JOIN Author ath ON aa.AuthorId = aa.Id
                            LEFT OUTER JOIN ArticleContent ac ON a.Id = ac.ArticleId
                            WHERE a.Id = @Id";
                var command = new CommandDefinition(sql, new
                {
                    Id = articleId
                }, cancellationToken: cancellationToken);
                await connection.QueryAsync<ArticleModel, AuthorModel, ArticleContentModel, ArticleModel>(command, (a, aa, ac) =>
                {
                    if (article == null)
                    {
                        article = a;
                    }

                    if (aa != null)
                    {
                        var author = article.Authors.SingleOrDefault(x => x.Id == aa.Id);
                        if (author == null)
                        {
                            article.Authors.Add(aa);
                        }
                    }

                    if (ac != null)
                    {
                        var content = article.Contents.SingleOrDefault(x => x.Id == ac.Id);
                        if (content == null)
                        {
                            article.Contents.Add(ac);
                        }
                    }

                    return article;
                });

                return article;
            }
        }
    }
}
