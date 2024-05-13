using Inshapardaz.Api.Controllers;
using Inshapardaz.Api.Extensions;
using Inshapardaz.Api.Helpers;
using Inshapardaz.Api.Mappings;
using Inshapardaz.Api.Views;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Adapters;
using Inshapardaz.Domain.Models.Library;

namespace Inshapardaz.Api.Converters
{
    public interface IRenderArticle
    {
        ArticleContentView Render(ArticleContentModel source, int libraryId, long articleId);

        ArticleView Render(ArticleModel source, int libraryId);

        PageView<ArticleView> Render(PageRendererArgs<ArticleModel, ArticleFilter, ArticleSortByType> source, int libraryId);
    }

    public class ArticleRenderer : IRenderArticle
    {
        private readonly IRenderLink _linkRenderer;
        private readonly IRenderAuthor _authorRenderer;
        private readonly IRenderCategory _categoryRenderer;
        private readonly IUserHelper _userHelper;

        public ArticleRenderer(IRenderLink linkRenderer, IRenderAuthor authorRenderer, IRenderCategory categoryRenderer,  IUserHelper userHelper)
        {
            _linkRenderer = linkRenderer;
            _authorRenderer = authorRenderer;
            _categoryRenderer = categoryRenderer;
            _userHelper = userHelper;
        }

        public PageView<ArticleView> Render(PageRendererArgs<ArticleModel, ArticleFilter, ArticleSortByType> source, int libraryId)
        {
            var page = new PageView<ArticleView>(source.Page.TotalCount, source.Page.PageSize, source.Page.PageNumber)
            {
                Data = source.Page.Data?.Select(x => Render(x, libraryId))
            };

            Dictionary<string, string> query = CreateQueryString(source, page);
            query.Add("pageNumber", (page.CurrentPageIndex).ToString());

            page.Links.Add(_linkRenderer.Render(new Link
            {
                ActionName = nameof(ArticleController.GetArticles),
                Method = HttpMethod.Get,
                Rel = RelTypes.Self,
                Parameters = new { libraryId = libraryId },
                QueryString = query
            }));

            if (_userHelper.IsWriter(libraryId) || _userHelper.IsAdmin || _userHelper.IsLibraryAdmin(libraryId))
            {
                page.Links.Add(_linkRenderer.Render(new Link
                {
                    ActionName = nameof(ArticleController.CreateArticle),
                    Method = HttpMethod.Post,
                    Rel = RelTypes.Create,
                    Parameters = new { libraryId = libraryId }
                }));
            }

            if (page.CurrentPageIndex < page.PageCount)
            {
                var pageQuery = CreateQueryString(source, page);
                pageQuery.Add("pageNumber", (page.CurrentPageIndex + 1).ToString());

                page.Links.Add(_linkRenderer.Render(new Link
                {
                    ActionName = nameof(ArticleController.GetArticles),
                    Method = HttpMethod.Get,
                    Rel = RelTypes.Next,
                    Parameters = new { libraryId = libraryId },
                    QueryString = pageQuery
                }));
            }

            if (page.PageCount > 1 && page.CurrentPageIndex > 1 && page.CurrentPageIndex <= page.PageCount)
            {
                var pageQuery = CreateQueryString(source, page);
                pageQuery.Add("pageNumber", (page.CurrentPageIndex - 1).ToString());

                page.Links.Add(_linkRenderer.Render(new Link
                {
                    ActionName = nameof(ArticleController.GetArticles),
                    Method = HttpMethod.Get,
                    Rel = RelTypes.Previous,
                    Parameters = new { libraryId = libraryId },
                    QueryString = pageQuery
                }));
            }

            return page;
        }

        public ArticleView Render(ArticleModel source, int libraryId)
        {
            var result = source.Map();
            var links = new List<LinkView>
            {
                _linkRenderer.Render(new Link
                {
                    ActionName = nameof(ArticleController.GetArticle),
                    Method = HttpMethod.Get,
                    Rel = RelTypes.Self,
                    Parameters = new { libraryId = libraryId, articleId = source.Id}
                })
            };


            if (source.ImageId.HasValue)
            {
                links.Add(_linkRenderer.Render(new Link
                {
                    ActionName = nameof(FileController.GetLibraryFile),
                    Method = HttpMethod.Get,
                    Rel = RelTypes.Image,
                    Parameters = new { libraryId = libraryId, fileId = source.ImageId.Value }
                }));
            }

            if (source.Contents != null && source.Contents.Any())
            {
                var contents = new List<ArticleContentView>();
                foreach (var content in source.Contents)
                {
                    contents.Add(Render(content, libraryId, source.Id));
                }

                result.Contents = contents;
            }

            if (source.PreviousArticle != null)
            {
                links.Add(_linkRenderer.Render(new Link
                {
                    ActionName = nameof(ArticleController.GetArticle),
                    Method = HttpMethod.Get,
                    Rel = RelTypes.Previous,
                    Parameters = new { libraryId = libraryId, articleId = source.PreviousArticle.Id }
                }));
            }

            if (source.NextArticle != null)
            {
                links.Add(_linkRenderer.Render(new Link
                {
                    ActionName = nameof(ArticleController.GetArticle),
                    Method = HttpMethod.Get,
                    Rel = RelTypes.Next,
                    Parameters = new { libraryId = libraryId, articleId = source.NextArticle.Id }
                }));
            }

            if (_userHelper.IsWriter(libraryId))
            {
                links.Add(_linkRenderer.Render(new Link
                {
                    ActionName = nameof(ArticleController.UpdateArticle),
                    Method = HttpMethod.Put,
                    Rel = RelTypes.Update,
                    Parameters = new { libraryId = libraryId, articleId = source.Id }
                }));

                links.Add(_linkRenderer.Render(new Link
                {
                    ActionName = nameof(ArticleController.DeleteArticle),
                    Method = HttpMethod.Delete,
                    Rel = RelTypes.Delete,
                    Parameters = new { libraryId = libraryId, articleId = source.Id }
                }));

                links.Add(_linkRenderer.Render(new Link
                {
                    ActionName = nameof(ArticleController.UpdateArticleContent),
                    Method = HttpMethod.Put,
                    Rel = RelTypes.AddContent,
                    Parameters = new { libraryId = libraryId, articleId = source.Id }
                }));

                links.Add(_linkRenderer.Render(new Link
                {
                    ActionName = nameof(ArticleController.UpdateArticleImage),
                    Method = HttpMethod.Put,
                    Rel = RelTypes.ImageUpload,
                    Parameters = new { libraryId = libraryId, articleId = source.Id }
                }));

                links.Add(_linkRenderer.Render(new Link
                {
                    ActionName = nameof(ArticleController.AssignArticleToUser),
                    Method = HttpMethod.Post,
                    Rel = RelTypes.Assign,
                    Parameters = new { libraryId = libraryId, articleId = source.Id }
                }));
            }

            if (_userHelper.IsAuthenticated)
            {
                if (source.Contents != null && source.Contents.Any())
                {
                    var contents = new List<ArticleContentView>();
                    foreach (var content in source.Contents)
                    {
                        contents.Add(Render(content, libraryId, source.Id));
                    }

                    result.Contents = contents;
                }

                if (source.IsFavorite)
                {
                    links.Add(_linkRenderer.Render(new Link
                    {
                        ActionName = nameof(ArticleController.RemoveArtiucleFromFavorites),
                        Method = HttpMethod.Delete,
                        Rel = RelTypes.RemoveFavorite,
                        Parameters = new { libraryId = libraryId, articleId = source.Id }
                    }));
                }
                else
                {
                    links.Add(_linkRenderer.Render(new Link
                    {
                        ActionName = nameof(ArticleController.AddArticleToFavorites),
                        Method = HttpMethod.Post,
                        Rel = RelTypes.CreateFavorite,
                        Parameters = new { libraryId = libraryId, articleId = source.Id }
                    }));
                }
            }

            if (source.Authors.Any())
            {
                var authors = new List<AuthorView>();
                foreach (var author in source.Authors)
                {
                    authors.Add(_authorRenderer.Render(author, libraryId));
                }

                result.Authors = authors;
            }

            if (source.Categories != null)
            {
                var categories = new List<CategoryView>();
                foreach (var category in source.Categories)
                {
                    categories.Add(_categoryRenderer.Render(category, libraryId));
                }

                result.Categories = categories;
            }

            result.Links = links;
            return result;
        }

        public ArticleContentView Render(ArticleContentModel source, int libraryId, long articleId)
        {
            var result = source.Map();

            var links = new List<LinkView>
            {
                _linkRenderer.Render(new Link
                {
                    ActionName = nameof(ArticleController.GetArticleContent),
                    Method = HttpMethod.Get,
                    Rel = RelTypes.Self,
                    Language = source.Language,
                    Parameters = new { libraryId = libraryId, articleId = articleId },
                    QueryString = new Dictionary<string, string>{{ "language",  source.Language}}
                }),
                _linkRenderer.Render(new Link
                {
                    ActionName = nameof(ArticleController.GetArticle),
                    Method = HttpMethod.Get,
                    Rel = RelTypes.Article,
                    Parameters = new { libraryId = libraryId, articleId = articleId }
                })
            };

            if (_userHelper.IsWriter(libraryId))
            {
                links.Add(_linkRenderer.Render(new Link
                {
                    ActionName = nameof(ArticleController.UpdateArticleContent),
                    Method = HttpMethod.Put,
                    Rel = RelTypes.Update,
                    Language = source.Language,
                    Parameters = new { libraryId = libraryId, articleId = articleId },
                    QueryString = new Dictionary<string, string> { { "language", source.Language } }
                }));

                links.Add(_linkRenderer.Render(new Link
                {
                    ActionName = nameof(ArticleController.DeleteArticleContent),
                    Method = HttpMethod.Delete,
                    Rel = RelTypes.Delete,
                    Language = source.Language,
                    Parameters = new { libraryId = libraryId, articleId = articleId },
                    QueryString = new Dictionary<string, string> { { "language", source.Language } }
                }));
            }

            result.Links = links;
            return result;
        }

        private static Dictionary<string, string> CreateQueryString(PageRendererArgs<ArticleModel, ArticleFilter, ArticleSortByType> source, PageView<ArticleView> page)
        {
            Dictionary<string, string> queryString = new Dictionary<string, string> {
                    { "pageSize", page.PageSize.ToString() }
                };

            if (!string.IsNullOrWhiteSpace(source.RouteArguments.Query))
            {
                queryString.Add("query", source.RouteArguments.Query);
            }

            if (source.Filters != null)
            {
                if (source.Filters.AuthorId.HasValue)
                    queryString.Add("authorid", source.Filters.AuthorId.Value.ToString());

                if (source.Filters.CategoryId.HasValue)
                    queryString.Add("categoryid", source.Filters.CategoryId.Value.ToString());

                if (source.Filters.Favorite.HasValue)
                    queryString.Add("favorite", bool.TrueString);

                if (source.Filters.Read.HasValue)
                    queryString.Add("read", bool.TrueString);

                if (source.Filters.Status != Domain.Models.EditingStatus.Completed)
                    queryString.Add("status", source.Filters.Status.ToDescription());

                if (source.Filters.Type != ArticleType.Unknown)
                    queryString.Add("type", source.Filters.Type.ToDescription());

                if (source.Filters.AssignmentStatus != AssignmentStatus.None)
                    queryString.Add("assignedfor", source.Filters.AssignmentStatus.ToDescription());
            }

            if (source.RouteArguments.SortBy != ArticleSortByType.Title)
                queryString.Add("sortby", source.RouteArguments.SortBy.ToDescription());

            if (source.RouteArguments.SortDirection != SortDirection.Ascending)
                queryString.Add("sortDirection", source.RouteArguments.SortDirection.ToDescription());
            return queryString;
        }
    }
}
