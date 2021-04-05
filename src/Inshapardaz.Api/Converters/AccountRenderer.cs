using Inshapardaz.Api.Controllers;
using Inshapardaz.Api.Helpers;
using Inshapardaz.Api.Mappings;
using Inshapardaz.Api.Views;
using Inshapardaz.Api.Views.Accounts;
using Inshapardaz.Domain.Models;
using Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http;
using System.Collections.Generic;
using System.Linq;

namespace Inshapardaz.Api.Converters
{
    public interface IRenderAccount
    {
        PageView<AccountView> Render(PageRendererArgs<AccountModel> source);

        AccountView Render(AccountModel series);

        IEnumerable<AccountLookupView> RenderLookup(IEnumerable<AccountModel> writers);
    }

    public class AccountRenderer : IRenderAccount
    {
        private readonly IRenderLink _linkRenderer;
        private readonly IUserHelper _userHelper;

        public AccountRenderer(IRenderLink linkRenderer, IUserHelper userHelper)
        {
            _linkRenderer = linkRenderer;
            _userHelper = userHelper;
        }

        public PageView<AccountView> Render(PageRendererArgs<AccountModel> source)
        {
            var page = new PageView<AccountView>(source.Page.TotalCount, source.Page.PageSize, source.Page.PageNumber)
            {
                Data = source.Page.Data?.Select(x => Render(x))
            };

            var links = new List<LinkView>
            {
                _linkRenderer.Render(new Link {
                    ActionName = nameof(AccountsController.GetAll),
                    Method = HttpMethod.Get,
                    Rel = RelTypes.Self,
                    QueryString = new Dictionary<string, string>()
                    {
                        { "pageNumber" , page.CurrentPageIndex.ToString() },
                        { "pageSize", page.PageSize.ToString() },
                        { "query", source.RouteArguments.Query }
                    }
                })
            };

            if (_userHelper.IsLibraryAdmin || _userHelper.IsAdmin)
            {
                links.Add(_linkRenderer.Render(new Link
                {
                    ActionName = nameof(AccountsController.Create),
                    Method = HttpMethod.Post,
                    Rel = RelTypes.Create
                }));
            }

            if (page.CurrentPageIndex < page.PageCount)
            {
                links.Add(_linkRenderer.Render(new Link
                {
                    ActionName = nameof(AccountsController.GetAll),
                    Method = HttpMethod.Get,
                    Rel = RelTypes.Next,
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
                    ActionName = nameof(AccountsController.GetAll),
                    Method = HttpMethod.Get,
                    Rel = RelTypes.Previous,
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

        public AccountView Render(AccountModel model)
        {
            var view = model.Map();

            view.Links.Add(_linkRenderer.Render(new Link
            {
                ActionName = nameof(AccountsController.GetById),
                Method = HttpMethod.Get,
                Rel = RelTypes.Self,
                Parameters = new { id = model.Id }
            }));

            //view.Links.Add(_linkRenderer.Render(new Link
            //{
            //    ActionName = nameof(BookController.GetBooks),
            //    Method = HttpMethod.Get,
            //    Rel = RelTypes.Books,
            //    Parameters = new { libraryId = libraryId },
            //    QueryString = new Dictionary<string, string>
            //    {
            //        { "seriesid", model.Id.ToString() }
            //    }
            //}));

            //if (!string.IsNullOrWhiteSpace(model.ImageUrl))
            //{
            //    view.Links.Add(new LinkView { Href = model.ImageUrl, Method = "GET", Rel = RelTypes.Image, Accept = MimeTypes.Jpg });
            //}
            //else if (model.ImageId.HasValue)
            //{
            //    view.Links.Add(_linkRenderer.Render(new Link
            //    {
            //        ActionName = nameof(FileController.GetFile),
            //        Method = HttpMethod.Get,
            //        Rel = RelTypes.Image,
            //        Parameters = new { fileId = model.ImageId.Value }
            //    }));
            //}

            if (_userHelper.IsLibraryAdmin || _userHelper.IsAdmin)
            {
                view.Links.Add(_linkRenderer.Render(new Link
                {
                    ActionName = nameof(AccountsController.Update),
                    Method = HttpMethod.Put,
                    Rel = RelTypes.Update,
                    Parameters = new { id = model.Id }
                }));

                view.Links.Add(_linkRenderer.Render(new Link
                {
                    ActionName = nameof(AccountsController.Delete),
                    Method = HttpMethod.Delete,
                    Rel = RelTypes.Delete,
                    Parameters = new { id = model.Id }
                }));

                view.Links.Add(_linkRenderer.Render(new Link
                {
                    ActionName = nameof(LibraryController.AddLibraryToAccount),
                    Method = HttpMethod.Post,
                    Rel = RelTypes.AddToLibrary,
                    Parameters = new { accountId = model.Id }
                }));
            }

            return view;
        }

        public IEnumerable<AccountLookupView> RenderLookup(IEnumerable<AccountModel> writers)
        {
            return writers.Select(w => w.MapToLookup());
        }
    }
}
