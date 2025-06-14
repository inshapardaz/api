﻿using Inshapardaz.Api.Controllers;
using Inshapardaz.Api.Extensions;
using Inshapardaz.Api.Mappings;
using Inshapardaz.Api.Views;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Adapters;
using Inshapardaz.Domain.Adapters.Repositories;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;

namespace Inshapardaz.Api.Converters;

public interface IRenderBook
{
    BookContentView Render(BookContentModel source, int libraryId);

    BookView Render(BookModel source, int libraryId, BookShelfModel bookShelf = null);

    PageView<BookView> Render(PageRendererArgs<BookModel, BookFilter> source, int id, BookShelfModel bookShelf = null);
    PageView<string> Render(int libraryId, PageRendererArgs<string> source);
}

public class BookRenderer : IRenderBook
{
    private readonly IRenderLink _linkRenderer;
    private readonly IRenderCategory _categoryRenderer;
    private readonly IRenderAuthor _authorRenderer;
    private readonly IFileStorage _fileStorage;
    private readonly IUserHelper _userHelper;

    public BookRenderer(IRenderLink linkRenderer, IRenderCategory categoryRenderer, IUserHelper userHelper, IRenderAuthor authorRenderer, IFileStorage fileStorage)
    {
        _linkRenderer = linkRenderer;
        _categoryRenderer = categoryRenderer;
        _userHelper = userHelper;
        _authorRenderer = authorRenderer;
        _fileStorage = fileStorage;
    }

    public PageView<BookView> Render(PageRendererArgs<BookModel, BookFilter> source, int libraryId, BookShelfModel bookShelf = null)
    {
        var page = new PageView<BookView>(source.Page.TotalCount, source.Page.PageSize, source.Page.PageNumber)
        {
            Data = source.Page.Data?.Select(x => Render(x, libraryId, bookShelf))
        };

        var links = new List<LinkView>();

        Dictionary<string, string> query = CreateQueryString(source, page);
        query.Add("pageNumber", (page.CurrentPageIndex).ToString());
        links.Add(_linkRenderer.Render(new Link
        {
            ActionName = nameof(BookController.GetBooks),
            Method = HttpMethod.Get,
            Rel = RelTypes.Self,
            Parameters = new { libraryId = libraryId },
            QueryString = query
        }));

        if (_userHelper.IsWriter(libraryId) || _userHelper.IsAdmin || _userHelper.IsLibraryAdmin(libraryId))
        {
            links.Add(_linkRenderer.Render(new Link
            {
                ActionName = nameof(BookController.CreateBook),
                Method = HttpMethod.Post,
                Rel = RelTypes.Create,
                Parameters = new { libraryId = libraryId }
            }));
        }

        if (page.CurrentPageIndex < page.PageCount)
        {
            var pageQuery = CreateQueryString(source, page);
            pageQuery.Add("pageNumber", (page.CurrentPageIndex + 1).ToString());

            links.Add(_linkRenderer.Render(new Link
            {
                ActionName = nameof(BookController.GetBooks),
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

            links.Add(_linkRenderer.Render(new Link
            {
                ActionName = nameof(BookController.GetBooks),
                Method = HttpMethod.Get,
                Rel = RelTypes.Previous,
                Parameters = new { libraryId = libraryId },
                QueryString = pageQuery
            }));
        }

        page.Links = links;
        return page;
    }

    public PageView<string> Render(int libraryId, PageRendererArgs<string> source)
    {
        Dictionary<string, string> GetQuery()
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            if (!string.IsNullOrWhiteSpace(source.RouteArguments.Query))
            {
                dictionary.Add("query", source.RouteArguments.Query);
            }

            return dictionary;
        }

        var page = new PageView<string>(source.Page.TotalCount, source.Page.PageSize, source.Page.PageNumber)
        {
            Data = source.Page.Data
        };

        var links = new List<LinkView>();

        var query = GetQuery();
        query.Add("pageNumber", (page.CurrentPageIndex).ToString());
        links.Add(_linkRenderer.Render(new Link
        {
            ActionName = nameof(BookController.GetPublishers),
            Method = HttpMethod.Get,
            Rel = RelTypes.Self,
            Parameters = new { libraryId = libraryId },
            QueryString = query
        }));
        

        if (page.CurrentPageIndex < page.PageCount)
        {
            var pageQuery = GetQuery();;
            pageQuery.Add("pageNumber", (page.CurrentPageIndex + 1).ToString());

            links.Add(_linkRenderer.Render(new Link
            {
                ActionName = nameof(BookController.GetPublishers),
                Method = HttpMethod.Get,
                Rel = RelTypes.Next,
                Parameters = new { libraryId = libraryId },
                QueryString = pageQuery
            }));
        }

        if (page.PageCount > 1 && page.CurrentPageIndex > 1 && page.CurrentPageIndex <= page.PageCount)
        {
            var pageQuery = GetQuery();
            pageQuery.Add("pageNumber", (page.CurrentPageIndex - 1).ToString());

            links.Add(_linkRenderer.Render(new Link
            {
                ActionName = nameof(BookController.GetPublishers),
                Method = HttpMethod.Get,
                Rel = RelTypes.Previous,
                Parameters = new { libraryId = libraryId },
                QueryString = pageQuery
            }));
        }

        page.Links = links;
        return page;
    }

    private static Dictionary<string, string> CreateQueryString(PageRendererArgs<BookModel, BookFilter> source, PageView<BookView> page)
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

            if (source.Filters.SeriesId.HasValue)
                queryString.Add("seriesid", source.Filters.SeriesId.Value.ToString());

            if (source.Filters.CategoryId.HasValue)
                queryString.Add("categoryid", source.Filters.CategoryId.Value.ToString());

            if (source.Filters.TagId.HasValue)
                queryString.Add("tagId", source.Filters.TagId.Value.ToString());

            if (source.Filters.BookShelfId.HasValue)
                queryString.Add("bookShelfId", source.Filters.BookShelfId.Value.ToString());

            if (source.Filters.Favorite.HasValue)
                queryString.Add("favorite", bool.TrueString);

            if (source.Filters.Read.HasValue)
                queryString.Add("read", bool.TrueString);

            if (source.Filters.Status != StatusType.Published)
                queryString.Add("status", source.Filters.Status.ToDescription());
        }

        if (source.RouteArguments.SortBy != BookSortByType.Title)
            queryString.Add("sortby", source.RouteArguments.SortBy.ToDescription());

        if (source.RouteArguments.SortDirection != SortDirection.Ascending)
            queryString.Add("sortDirection", source.RouteArguments.SortDirection.ToDescription());
        return queryString;
    }

    public BookView Render(BookModel source, int libraryId, BookShelfModel bookShelf = null)
    {
        var result = source.Map();
        var links = new List<LinkView>
        {
            _linkRenderer.Render(new Link {
                ActionName = nameof(BookController.GetBookById),
                Method = HttpMethod.Get,
                Rel = RelTypes.Self,
                Parameters = new { libraryId = libraryId, bookId = source.Id }
            }),
            _linkRenderer.Render(new Link {
                ActionName = nameof(AuthorController.GetAuthorById),
                Method = HttpMethod.Get,
                Rel = RelTypes.Author,
                Parameters = new { libraryId = libraryId, authorId = source.Authors.First().Id }
            }),
            _linkRenderer.Render(new Link {
                ActionName = nameof(ChapterController.GetChaptersByBook),
                Method = HttpMethod.Get,
                Rel = RelTypes.Chapters,
                Parameters = new { libraryId = libraryId, bookId = source.Id }
            }),
            _linkRenderer.Render(new Link
            {
                ActionName = nameof(BookPageController.GetPagesByBook),
                Method = HttpMethod.Get,
                Rel = RelTypes.Pages,
                Parameters = new { libraryId = libraryId, bookId = source.Id }
            })
        };

        if (source.SeriesId.HasValue)
        {
            links.Add(_linkRenderer.Render(new Link
            {
                ActionName = nameof(SeriesController.GetSeriesById),
                Method = HttpMethod.Get,
                Rel = RelTypes.Series,
                Parameters = new { libraryId = libraryId, seriesId = source.SeriesId }
            }));
        }

        if (bookShelf is not null)
        {
            result.BookShelf = bookShelf.Map();

            links.Add(_linkRenderer.Render(new Link
            {
                ActionName = nameof(BookShelfController.DeleteBookInBookShelf),
                Method = HttpMethod.Delete,
                Rel = RelTypes.RemoveBookFromBookShelf,
                Parameters = new { libraryId = libraryId, bookShelfId = bookShelf.Id, bookId = source.Id }
            }));
        }

        if (!string.IsNullOrWhiteSpace(source.ImageUrl) && _fileStorage.SupportsPublicLink)
        {
            links.Add(new LinkView
            {
                Href = _fileStorage.GetPublicUrl(source.ImageUrl),
                Method = "GET",
                Rel = RelTypes.Image,
                Accept = MimeTypes.Jpg
            });
        }
        else if (source.ImageId.HasValue)
        {
            links.Add(_linkRenderer.Render(new Link
            {
                ActionName = nameof(FileController.GetLibraryFile),
                Method = HttpMethod.Get,
                Rel = RelTypes.Image,
                Parameters = new { libraryId = libraryId, fileId = source.ImageId.Value }
            }));
        }

        if (_userHelper.IsWriter(libraryId) || _userHelper.IsAdmin || _userHelper.IsLibraryAdmin(libraryId))
        {
            links.Add(_linkRenderer.Render(new Link
            {
                ActionName = nameof(BookController.UpdateBook),
                Method = HttpMethod.Put,
                Rel = RelTypes.Update,
                Parameters = new { libraryId = libraryId, bookId = source.Id }
            }));
            links.Add(_linkRenderer.Render(new Link
            {
                ActionName = nameof(BookController.DeleteBook),
                Method = HttpMethod.Delete,
                Rel = RelTypes.Delete,
                Parameters = new { libraryId = libraryId, bookId = source.Id }
            }));
            links.Add(_linkRenderer.Render(new Link
            {
                ActionName = nameof(BookController.UpdateBookImage),
                Method = HttpMethod.Put,
                Rel = RelTypes.ImageUpload,
                Parameters = new { libraryId = libraryId, bookId = source.Id }
            }));
            links.Add(_linkRenderer.Render(new Link
            {
                ActionName = nameof(BookController.CreateBookContent),
                Method = HttpMethod.Post,
                Rel = RelTypes.AddFile,
                Parameters = new { libraryId = libraryId, bookId = source.Id }
            }));
            links.Add(_linkRenderer.Render(new Link
            {
                ActionName = nameof(ChapterController.CreateChapter),
                Method = HttpMethod.Post,
                Rel = RelTypes.CreateChapter,
                Parameters = new { libraryId = libraryId, bookId = source.Id }
            }));
            links.Add(_linkRenderer.Render(new Link
            {
                ActionName = nameof(BookPageController.CreatePage),
                Method = HttpMethod.Post,
                Rel = RelTypes.AddPages,
                Parameters = new { libraryId = libraryId, bookId = source.Id }
            }));

            links.Add(_linkRenderer.Render(new Link
            {
                ActionName = nameof(BookPageController.UploadPages),
                Method = HttpMethod.Post,
                Rel = RelTypes.MultiCreate,
                Parameters = new { libraryId = libraryId, bookId = source.Id }
            }));
        }

        if (_userHelper.IsAdmin || _userHelper.IsLibraryAdmin(libraryId))
        {
            links.Add(_linkRenderer.Render(new Link
            {
                ActionName = nameof(BookController.BindBook),
                Method = HttpMethod.Get,
                Rel = RelTypes.Download,
                Parameters = new { libraryId = libraryId, bookId = source.Id }
            }));

            links.Add(_linkRenderer.Render(new Link
            {
                ActionName = nameof(BookController.PublishBook),
                Method = HttpMethod.Post,
                Rel = RelTypes.Publish,
                Parameters = new { libraryId = libraryId, bookId = source.Id }
            }));
        }

        if (_userHelper.IsAuthenticated)
        {
            if (source.IsFavorite)
            {
                links.Add(_linkRenderer.Render(new Link
                {
                    ActionName = nameof(BookController.RemoveBookFromFavorites),
                    Method = HttpMethod.Delete,
                    Rel = RelTypes.RemoveFavorite,
                    Parameters = new { libraryId = libraryId, bookId = source.Id }
                }));
            }
            else
            {
                links.Add(_linkRenderer.Render(new Link
                {
                    ActionName = nameof(BookController.AddBookToFavorites),
                    Method = HttpMethod.Post,
                    Rel = RelTypes.CreateFavorite,
                    Parameters = new { libraryId = libraryId, bookId = source.Id }
                }));
            }
        }

        result.Links = links;

        if (source.Authors.Any())
        {
            var authors = new List<AuthorView>();
            foreach (var author in source.Authors)
            {
                authors.Add(_authorRenderer.Render(author, source.LibraryId));
            }

            result.Authors = authors;
        }

        if (source.Categories != null)
        {
            var categories = new List<CategoryView>();
            foreach (var category in source.Categories)
            {
                categories.Add(_categoryRenderer.Render(category, source.LibraryId));
            }

            result.Categories = categories;
        }

        if (source.Contents.Any())
        {
            var contents = new List<BookContentView>();
            foreach (var content in source.Contents)
            {
                contents.Add(Render(content, source.LibraryId));
            }

            result.Contents = contents;
        }

        return result;
    }

    public BookContentView Render(BookContentModel source, int libraryId)
    {
        var result = source.Map();

        var links = new List<LinkView>
        {
            _linkRenderer.Render(new Link {
                ActionName = nameof(BookController.GetBookContent),
                Method = HttpMethod.Get,
                Rel = RelTypes.Self,
                Language = source.Language,
                MimeType = source.MimeType,
                Parameters = new { libraryId = libraryId, bookId = source.BookId, contentId = source.Id }
            }),
            _linkRenderer.Render(new Link {
                ActionName = nameof(BookController.GetBookById),
                Method = HttpMethod.Get,
                Rel = RelTypes.Book,
                Parameters = new { libraryId = libraryId, bookId = source.BookId }
            })
        };

        if (!string.IsNullOrWhiteSpace(source.ContentUrl) && _fileStorage.SupportsPublicLink)

        {
            links.Add(new LinkView
            {
                Href = _fileStorage.GetPublicUrl(source.ContentUrl),
                Method = "GET",
                Rel = RelTypes.Download,
                Accept = source.MimeType,
                AcceptLanguage = source.Language
            });
        }
        else
        {
            links.Add(_linkRenderer.Render(new Link
            {
                ActionName = nameof(FileController.GetLibraryFile),
                Method = HttpMethod.Get,
                Rel = RelTypes.Download,
                Language = source.Language,
                MimeType = source.MimeType,
                Parameters = new { libraryId = libraryId, fileId = source.FileId }
            }));
        }

        if (_userHelper.IsWriter(libraryId) || _userHelper.IsAdmin || _userHelper.IsLibraryAdmin(libraryId))
        {
            links.Add(_linkRenderer.Render(new Link
            {
                ActionName = nameof(BookController.UpdateBookContent),
                Method = HttpMethod.Put,
                Rel = RelTypes.Update,
                Language = source.Language,
                MimeType = source.MimeType,
                Parameters = new { libraryId = libraryId, bookId = source.BookId, contentId = source.Id }
            }));

            links.Add(_linkRenderer.Render(new Link
            {
                ActionName = nameof(BookController.DeleteBookContent),
                Method = HttpMethod.Delete,
                Rel = RelTypes.Delete,
                Language = source.Language,
                MimeType = source.MimeType,
                Parameters = new { libraryId = libraryId, bookId = source.BookId, contentId = source.Id }
            }));
        }

        result.Links = links;
        return result;
    }
}
