using Inshapardaz.Api.Controllers;
using Inshapardaz.Api.Mappings;
using Inshapardaz.Api.Views;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Adapters;
using Inshapardaz.Domain.Models.Library;
using Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http;
using System.Collections.Generic;
using System.Linq;

namespace Inshapardaz.Api.Converters
{
    public interface IRenderBookPage
    {
        BookPageView Render(BookPageModel source, int libraryId, int bookId);

        PageView<BookPageView> Render(PageRendererArgs<BookPageModel> source, int libraryId, int bookId);

        LinkView RenderImageLink(int libraryId, int bookId, int pageNumber);
    }

    public class BookPageRenderer : IRenderBookPage
    {
        private readonly IRenderLink _linkRenderer;
        private readonly IUserHelper _userHelper;

        public BookPageRenderer(IRenderLink linkRenderer, IUserHelper userHelper)
        {
            _linkRenderer = linkRenderer;
            _userHelper = userHelper;
        }

        public PageView<BookPageView> Render(PageRendererArgs<BookPageModel> source, int libraryId, int bookId)
        {
            var page = new PageView<BookPageView>(source.Page.TotalCount, source.Page.PageSize, source.Page.PageNumber)
            {
                Data = source.Page.Data?.Select(x => Render(x, libraryId, bookId))
            };

            var links = new List<LinkView>();

            links.Add(_linkRenderer.Render(new Link
            {
                ActionName = nameof(BookPageController.GetPagesByBook),
                Method = HttpMethod.Get,
                Rel = RelTypes.Self,
                Parameters = new { libraryId = libraryId, bookId = bookId },
                QueryString = new Dictionary<string, string>()
                {
                    { "pageNumber" , page.CurrentPageIndex.ToString() },
                    { "pageSize", page.PageSize.ToString() }
                }
            }));

            if (_userHelper.IsWriter || _userHelper.IsAdmin || _userHelper.IsLibraryAdmin)
            {
                links.Add(_linkRenderer.Render(new Link
                {
                    ActionName = nameof(BookPageController.CreatePage),
                    Method = HttpMethod.Post,
                    Rel = RelTypes.Create,
                    Parameters = new { libraryId = libraryId, bookId = bookId }
                }));
            }

            if (page.CurrentPageIndex < page.PageCount)
            {
                links.Add(_linkRenderer.Render(new Link
                {
                    ActionName = nameof(BookPageController.GetPagesByBook),
                    Method = HttpMethod.Get,
                    Rel = RelTypes.Next,
                    Parameters = new { libraryId = libraryId, bookId = bookId },
                    QueryString = new Dictionary<string, string>()
                    {
                        { "pageNumber" , (page.CurrentPageIndex + 1).ToString() },
                        { "pageSize", page.PageSize.ToString() }
                    }
                }));
            }

            if (page.PageCount > 1 && page.CurrentPageIndex > 1 && page.CurrentPageIndex <= page.PageCount)
            {
                links.Add(_linkRenderer.Render(new Link
                {
                    ActionName = nameof(BookPageController.GetPagesByBook),
                    Method = HttpMethod.Get,
                    Rel = RelTypes.Next,
                    Parameters = new { libraryId = libraryId, bookId = bookId },
                    QueryString = new Dictionary<string, string>()
                    {
                        { "pageNumber" , (page.CurrentPageIndex - 1).ToString() },
                        { "pageSize", page.PageSize.ToString() }
                    }
                }));
            }

            page.Links = links;
            return page;
        }

        public BookPageView Render(BookPageModel source, int libraryId, int bookId)
        {
            var result = source.Map();
            var links = new List<LinkView>
                    {
                        _linkRenderer.Render(new Link
                        {
                            ActionName = nameof(BookPageController.GetPagesByIndex),
                            Method = HttpMethod.Get,
                            Rel = RelTypes.Self,
                            Parameters = new { libraryId = libraryId, bookId = bookId, pageNumber = source.PageNumber }
                        }),
                        _linkRenderer.Render(new Link
                        {
                            ActionName = nameof(BookController.GetBookById),
                            Method = HttpMethod.Get,
                            Rel = RelTypes.Book,
                            Parameters = new { libraryId = libraryId, bookId = bookId }
                        })
                    };
            if (source.ImageId.HasValue)
            {
                links.Add(RenderImageLink(libraryId, bookId, source.PageNumber));
            }

            if (_userHelper.IsWriter || _userHelper.IsLibraryAdmin || _userHelper.IsAdmin)
            {
                links.Add(_linkRenderer.Render(new Link
                {
                    ActionName = nameof(BookPageController.UpdatePage),
                    Method = HttpMethod.Put,
                    Rel = RelTypes.Update,
                    Parameters = new { libraryId = libraryId, bookId = bookId, pageNumber = source.PageNumber }
                }));

                links.Add(_linkRenderer.Render(new Link
                {
                    ActionName = nameof(BookPageController.DeletePage),
                    Method = HttpMethod.Delete,
                    Rel = RelTypes.Delete,
                    Parameters = new { libraryId = libraryId, bookId = bookId, pageNumber = source.PageNumber }
                }));
            }

            result.Links = links;
            return result;
        }

        public LinkView RenderImageLink(int libraryId, int bookId, int pageNumber) =>
            _linkRenderer.Render(new Link
            {
                ActionName = nameof(BookPageController.GetPageImage),
                Method = HttpMethod.Get,
                Rel = RelTypes.Image,
                Parameters = new { libraryId = libraryId, bookId = bookId, pageNumber = pageNumber }
            });
    }
}
