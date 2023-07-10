using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;

namespace Inshapardaz.Database.SqlServer.Repositories.Library
{
    public class ArticleRepository : IArticleRepository
    {
        public Task<ArticleModel> AddArticle(int libraryId, ArticleModel article, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
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
    }
}
