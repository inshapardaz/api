﻿using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Adapters.Repositories.Library
{
    public interface IArticleRepository
    {
        Task<Page<ArticleModel>> GetArticles(int libraryId, int pageNumber, int pageSize, int? accountId, ArticleFilter filter, ArticleSortByType sortBy, SortDirection sortDirection, CancellationToken cancellationToken);
        Task<Page<ArticleModel>> SearchArticles(int libraryId, string query, int pageNumber, int pageSize, int? accountId, ArticleFilter filter, ArticleSortByType sortBy, SortDirection sortDirection, CancellationToken cancellationToken);

        Task<ArticleModel> GetArticle(int libraryId, long articleId, CancellationToken cancellationToken);

        Task<ArticleModel> AddArticle(int libraryId, ArticleModel article, CancellationToken cancellationToken);

        Task UpdateArticle(int libraryId, long articleId, ArticleModel article, CancellationToken cancellationToken);

        Task<IEnumerable<ArticleContentModel>> GetArticleContents(int libraryId, long articleId, CancellationToken cancellationToken);

        Task DeleteArticle(int libraryId, long articleId, CancellationToken cancellationToken);

        Task<ArticleContentModel> GetArticleContentById(int libraryId, long articleId, string language, CancellationToken cancellationToken);

        Task<ArticleContentModel> AddArticleContent(int libraryId, long articleId, string language, string content, CancellationToken cancellationToken);

        Task<ArticleContentModel> UpdateArticleContent(int libraryId, long articleId, string language, string content, CancellationToken cancellationToken);

        Task<ArticleContentModel> GetArticleContent(int libraryId, long articleId, string language, CancellationToken cancellationToken);

        Task DeleteArticleContent(int libraryId, long articleId, string language, CancellationToken cancellationToken);
        Task<ArticleModel> UpdateWriterAssignment(int libraryId, long articleId, int? accountId, CancellationToken cancellationToken);
        Task<ArticleModel> UpdateReviewerAssignment(int libraryId, long articleId, int? accountId, CancellationToken cancellationToken);
    }
}
