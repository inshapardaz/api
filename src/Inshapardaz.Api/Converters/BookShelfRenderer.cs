using Amazon.Runtime.Internal.Transform;
using Inshapardaz.Api.Controllers;
using Inshapardaz.Api.Helpers;
using Inshapardaz.Api.Mappings;
using Inshapardaz.Api.Views;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Domain.Repositories;
using Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http;
using System.Collections.Generic;
using System.Linq;

namespace Inshapardaz.Api.Converters
{
    public interface IRenderBookSelf
    {
        PageView<BookShelfView> Render(PageRendererArgs<BookShelfModel> source, int libraryId);

        BookShelfView Render(BookShelfModel source, int libraryId);
    }

    public class BookShelfRenderer : IRenderBookSelf
    {
        private readonly IRenderLink _linkRenderer;
        private readonly IUserHelper _userHelper;
        private readonly IFileStorage _fileStorage;

        public BookShelfRenderer(IRenderLink linkRenderer, IUserHelper userHelper, IFileStorage fileStorage)
        {
            _linkRenderer = linkRenderer;
            _userHelper = userHelper;
            _fileStorage = fileStorage;
        }

        public PageView<BookShelfView> Render(PageRendererArgs<BookShelfModel> source, int libraryId)
        {
            var page = new PageView<BookShelfView>(source.Page.TotalCount, source.Page.PageSize, source.Page.PageNumber)
            {
                Data = source.Page.Data?.Select(x => Render(x, libraryId))
            };

            var links = new List<LinkView>
            {
                _linkRenderer.Render(new Link {
                    ActionName = nameof(BookShelfController.GetBookShelves),
                    Method = HttpMethod.Get,
                    Rel = RelTypes.Self,
                    Parameters = new { libraryId = libraryId },
                    QueryString = new Dictionary<string, string>()
                    {
                        { "pageNumber" , page.CurrentPageIndex.ToString() },
                        { "pageSize", page.PageSize.ToString() },
                        { "query", source.RouteArguments.Query }
                    }
                })
            };

            if (_userHelper.IsWriter(libraryId) || _userHelper.IsAdmin || _userHelper.IsLibraryAdmin(libraryId))
            {
                links.Add(_linkRenderer.Render(new Link
                {
                    ActionName = nameof(BookShelfController.CreateBookShelf),
                    Method = HttpMethod.Post,
                    Rel = RelTypes.Create,
                    Parameters = new { libraryId = libraryId }
                }));
            }

            if (page.CurrentPageIndex < page.PageCount)
            {
                links.Add(_linkRenderer.Render(new Link
                {
                    ActionName = nameof(BookShelfController.GetBookShelves),
                    Method = HttpMethod.Get,
                    Rel = RelTypes.Next,
                    Parameters = new { libraryId = libraryId },
                    QueryString = new Dictionary<string, string>()
                    {
                        { "pageNumber" , (page.CurrentPageIndex + 1).ToString() },
                        { "pageSize", page.PageSize.ToString() },
                        { "query", source.RouteArguments.Query }
                    }
                }));
            }

            if (page.PageCount > 1 && page.CurrentPageIndex > 1 && page.CurrentPageIndex <= page.PageCount)
            {
                links.Add(_linkRenderer.Render(new Link
                {
                    ActionName = nameof(BookShelfController.GetBookShelves),
                    Method = HttpMethod.Get,
                    Rel = RelTypes.Previous,
                    Parameters = new { libraryId = libraryId },
                    QueryString = new Dictionary<string, string>()
                    {
                        { "pageNumber" , (page.CurrentPageIndex - 1).ToString() },
                        { "pageSize", page.PageSize.ToString() },
                        { "query", source.RouteArguments.Query }
                    }
                }));
            }

            page.Links = links;
            return page;
        }

        public BookShelfView Render(BookShelfModel source, int libraryId)
        {
            var result = source.Map();

            var links = new List<LinkView>
            {
                _linkRenderer.Render(new Link
                {
                    ActionName = nameof(BookShelfController.GetBookShelf),
                    Method = HttpMethod.Get,
                    Rel = RelTypes.Self,
                    Parameters = new { libraryId = libraryId, bookShelfId = source.Id }
                }),
                _linkRenderer.Render(new Link
                {
                    ActionName = nameof(BookController.GetBooks),
                    Method = HttpMethod.Get,
                    Rel = RelTypes.Books,
                    Parameters = new { libraryId = libraryId },
                    QueryString = new Dictionary<string, string>
                    {
                        { "bookShelfId", source.Id.ToString() }
                    }
                })
            };

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
                    ActionName = nameof(FileController.GetFile),
                    Method = HttpMethod.Get,
                    Rel = RelTypes.Image,
                    Parameters = new { fileId = source.ImageId.Value }
                }));
            }

            if (_userHelper.Account.Id == source.AccountId)
            {
                links.Add(_linkRenderer.Render(new Link
                {
                    ActionName = nameof(BookShelfController.UpdateBookShelf),
                    Method = HttpMethod.Put,
                    Rel = RelTypes.Update,
                    Parameters = new { libraryId = libraryId, bookShelfId = source.Id }
                }));

                links.Add(_linkRenderer.Render(new Link
                {
                    ActionName = nameof(BookShelfController.UpdateBookShelfImage),
                    Method = HttpMethod.Put,
                    Rel = RelTypes.ImageUpload,
                    Parameters = new { libraryId = libraryId, bookShelfId = source.Id }
                }));
            }

            if (_userHelper.Account.Id == source.AccountId || _userHelper.IsAdmin || _userHelper.IsLibraryAdmin(libraryId))
            {
                links.Add(_linkRenderer.Render(new Link
                {
                    ActionName = nameof(BookShelfController.DeleteBookShelf),
                    Method = HttpMethod.Delete,
                    Rel = RelTypes.Delete,
                    Parameters = new { libraryId = libraryId, bookShelfId = source.Id }
                }));

                links.Add(_linkRenderer.Render(new Link
                {
                    ActionName = nameof(BookShelfController.AddBookInBookShelf),
                    Method = HttpMethod.Delete,
                    Rel = RelTypes.AddBook,
                    Parameters = new { libraryId = libraryId, bookShelfId = source.Id }
                }));
            }


            result.Links = links;
            return result;
        }
    }
}
