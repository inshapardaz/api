using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Api.Views;
using System.Collections.Generic;
using Inshapardaz.Api.Controllers;
using Inshapardaz.Domain.Adapters;
using Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http;
using System.Linq;
using Inshapardaz.Api.Helpers;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.InteropServices;

namespace Inshapardaz.Api.Converters
{
    public interface IRenderLibrary
    {
        PageView<LibraryView> Render(PageRendererArgs<LibraryModel> source);

        LibraryView Render(LibraryModel model);
    }

    public class LibraryRenderer : IRenderLibrary
    {
        private readonly IRenderLink _linkRenderer;
        private readonly IUserHelper _userHelper;

        public LibraryRenderer(IRenderLink linkRenderer, IUserHelper userHelper)
        {
            _linkRenderer = linkRenderer;
            _userHelper = userHelper;
        }

        public PageView<LibraryView> Render(PageRendererArgs<LibraryModel> source)
        {
            var page = new PageView<LibraryView>(source.Page.TotalCount, source.Page.PageSize, source.Page.PageNumber)
            {
                Data = source.Page.Data?.Select(x => Render(x))
            };

            var queryString = new Dictionary<string, string>()
            {
                { "pageNumber" , page.CurrentPageIndex.ToString() },
                { "pageSize", page.PageSize.ToString() },
                { "query", source.RouteArguments.Query }
            };

            if (source.RouteArguments.AccountId.HasValue)
            {
                queryString.Add("accountId", source.RouteArguments.AccountId.Value.ToString());
            }

            var links = new List<LinkView>
            {
                _linkRenderer.Render(new Link {
                    ActionName = nameof(LibraryController.GetLibraries),
                    Method = HttpMethod.Get,
                    Rel = RelTypes.Self,
                    Parameters = new object(),
                    QueryString = queryString
                })
            };

            if (_userHelper.IsAdmin)
            {
                links.Add(_linkRenderer.Render(new Link
                {
                    ActionName = nameof(LibraryController.CreateLibrary),
                    Method = HttpMethod.Post,
                    Rel = RelTypes.Create
                }));
            }

            if (page.CurrentPageIndex < page.PageCount)
            {
                var queryStringNext = new Dictionary<string, string>()
                {
                    { "pageNumber" , (page.CurrentPageIndex + 1).ToString() },
                    { "pageSize", page.PageSize.ToString() },
                    { "query", source.RouteArguments.Query }
                };

                if (source.RouteArguments.AccountId.HasValue)
                {
                    queryStringNext.Add("accountId", source.RouteArguments.AccountId.Value.ToString());
                }

                links.Add(_linkRenderer.Render(new Link
                {
                    ActionName = nameof(LibraryController.GetLibraries),
                    Method = HttpMethod.Get,
                    Rel = RelTypes.Next,
                    Parameters = new object(),
                    QueryString = queryStringNext
                }));
            }

            if (page.PageCount > 1 && page.CurrentPageIndex > 1 && page.CurrentPageIndex <= page.PageCount)
            {
                var queryStringPrev = new Dictionary<string, string>()
                {
                    { "pageNumber" , (page.CurrentPageIndex - 1).ToString() },
                    { "pageSize", page.PageSize.ToString() },
                    { "query", source.RouteArguments.Query }
                };

                if (source.RouteArguments.AccountId.HasValue)
                {
                    queryStringPrev.Add("accountId", source.RouteArguments.AccountId.Value.ToString());
                }

                links.Add(_linkRenderer.Render(new Link
                {
                    ActionName = nameof(LibraryController.GetLibraries),
                    Method = HttpMethod.Get,
                    Rel = RelTypes.Previous,
                    QueryString = queryStringPrev
                }));
            }

            page.Links = links;
            return page;
        }

        public LibraryView Render(LibraryModel model)
        {
            var links = new List<LinkView>
            {
                _linkRenderer.Render(new Link
                {
                    ActionName = nameof(LibraryController.GetLibraryById),
                    Method = HttpMethod.Get,
                    Rel = RelTypes.Self,
                    Parameters = new { libraryId = model.Id }
                }),

                _linkRenderer.Render(new Link
                {
                    ActionName = nameof(AuthorController.GetAuthors),
                    Method = HttpMethod.Get,
                    Rel = RelTypes.Authors,
                    Parameters = new { libraryId = model.Id }
                }),
                _linkRenderer.Render(new Link
                {
                    ActionName = nameof(CategoryController.GetCategories),
                    Method = HttpMethod.Get,
                    Rel = RelTypes.Categories,
                    Parameters = new { libraryId = model.Id }
                }),
                _linkRenderer.Render(new Link
                {
                    ActionName = nameof(SeriesController.GetSereies),
                    Method = HttpMethod.Get,
                    Rel = RelTypes.Series,
                    Parameters = new { libraryId = model.Id }
                }),
                _linkRenderer.Render(new Link
                {
                    ActionName = nameof(BookController.GetBooks),
                    Method = HttpMethod.Get,
                    Rel = RelTypes.Books,
                    Parameters = new { libraryId = model.Id }
                })
            };

            if (model.SupportsPeriodicals)
            {
                links.Add(_linkRenderer.Render(new Link
                {
                    ActionName = nameof(PeriodicalController.GetPeriodicals),
                    Method = HttpMethod.Get,
                    Rel = RelTypes.Periodicals,
                    Parameters = new { libraryId = model.Id }
                }));
            }

            if (_userHelper.IsAuthenticated)
            {
                links.Add(_linkRenderer.Render(new Link
                {
                    ActionName = nameof(BookController.GetBooks),
                    Method = HttpMethod.Get,
                    Rel = RelTypes.Recents,
                    Parameters = new { libraryId = model.Id },
                    QueryString = new Dictionary<string, string> { { "read", bool.TrueString } }
                }));
            }

            if (_userHelper.IsWriter || _userHelper.IsAdmin || _userHelper.IsLibraryAdmin)
            {
                links.Add(_linkRenderer.Render(new Link
                {
                    ActionName = nameof(BookController.CreateBook),
                    Method = HttpMethod.Post,
                    Rel = RelTypes.CreateBook,
                    Parameters = new { libraryId = model.Id }
                }));

                links.Add(_linkRenderer.Render(new Link
                {
                    ActionName = nameof(AuthorController.CreateAuthor),
                    Method = HttpMethod.Post,
                    Rel = RelTypes.CreateAuthor,
                    Parameters = new { libraryId = model.Id }
                }));

                links.Add(_linkRenderer.Render(new Link
                {
                    ActionName = nameof(SeriesController.CreateSeries),
                    Method = HttpMethod.Post,
                    Rel = RelTypes.CreateSeries,
                    Parameters = new { libraryId = model.Id }
                }));
            }

            if (_userHelper.IsLibraryAdmin || _userHelper.IsAdmin)
            {
                links.Add(_linkRenderer.Render(new Link
                {
                    ActionName = nameof(LibraryController.UpdateLibrary),
                    Method = HttpMethod.Put,
                    Rel = RelTypes.Update,
                    Parameters = new { libraryId = model.Id }
                }));

                links.Add(_linkRenderer.Render(new Link
                {
                    ActionName = nameof(CategoryController.CreateCategory),
                    Method = HttpMethod.Post,
                    Rel = RelTypes.CreateCategory,
                    Parameters = new { libraryId = model.Id }
                }));
            }

            if (_userHelper.IsAdmin)
            {
                links.Add(_linkRenderer.Render(new Link
                {
                    ActionName = nameof(LibraryController.DeleteLibrary),
                    Method = HttpMethod.Delete,
                    Rel = RelTypes.Delete,
                    Parameters = new { libraryId = model.Id }
                }));
            }

            return new LibraryView
            {
                Id = model.Id,
                Name = model.Name,
                Language = model.Language,
                SupportsPeriodicals = model.SupportsPeriodicals,
                PrimaryColor = model.PrimaryColor,
                SecondaryColor = model.SecondaryColor,
                Links = links
            };
        }
    }
}
