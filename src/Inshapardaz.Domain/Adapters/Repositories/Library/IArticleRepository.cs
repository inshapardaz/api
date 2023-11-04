using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Adapters.Repositories.Library
{
    public interface IArticleRepository
    {
        Task<Page<ArticleModel>> GetArticles(int libraryId, string query, int pageNumber, int pageSize, int? accountId, ArticleFilter filter, ArticleSortByType sortBy, SortDirection sortDirection, CancellationToken cancellationToken);

        Task<ArticleModel> GetArticle(int libraryId, long articleId, CancellationToken cancellationToken);

        Task<ArticleModel> AddArticle(int libraryId, ArticleModel article, int? accountId, CancellationToken cancellationToken);

        Task<ArticleModel> UpdateArticle(int libraryId, long articleId, ArticleModel article, CancellationToken cancellationToken);

        Task<IEnumerable<ArticleContentModel>> GetArticleContents(int libraryId, long articleId, CancellationToken cancellationToken);

        Task DeleteArticle(int libraryId, long articleId, CancellationToken cancellationToken);

        #region Content
        Task<ArticleContentModel> AddArticleContent(int libraryId, long articleId, string language, string content, CancellationToken cancellationToken);

        Task<ArticleContentModel> UpdateArticleContent(int libraryId, long articleId, string language, string content, CancellationToken cancellationToken);

        Task<ArticleContentModel> GetArticleContent(int libraryId, long articleId, string language, CancellationToken cancellationToken);

        Task DeleteArticleContent(int libraryId, long articleId, string language, CancellationToken cancellationToken);
        #endregion
        Task<ArticleModel> UpdateWriterAssignment(int libraryId, long articleId, int? accountId, CancellationToken cancellationToken);
        
        Task<ArticleModel> UpdateReviewerAssignment(int libraryId, long articleId, int? accountId, CancellationToken cancellationToken);

        Task UpdateArticleImage(int libraryId, long articleId, int imageId, CancellationToken cancellationToken);

        #region Faviorite 
        Task AddArticleToFavorites(int libraryId, int? accountId, long articleId, CancellationToken cancellationToken);
        Task RemoveArticleFromFavorites(int libraryId, int? accountId, long articleId, CancellationToken cancellationToken);
        #endregion
    }
}
