using Inshapardaz.Api.Controllers;
using Inshapardaz.Api.Helpers;
using Inshapardaz.Api.Mappings;
using Inshapardaz.Api.Views;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Adapters;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http;
using System.Collections.Generic;
using System.Linq;

namespace Inshapardaz.Api.Converters
{
    public interface IRenderCorrection
    {
        PageView<CorrectionView> Render(PageRendererArgs<CorrectionModel> source, string language, string profile);

        CorrectionView Render(CorrectionModel correction);
    }

    public class CorrectionRenderer : IRenderCorrection
    {
        private readonly IRenderLink _linkRenderer;
        private readonly IUserHelper _userHelper;

        public CorrectionRenderer(IRenderLink linkRenderer, IUserHelper userHelper)
        {
            _linkRenderer = linkRenderer;
            _userHelper = userHelper;
        }

        public PageView<CorrectionView> Render(PageRendererArgs<CorrectionModel> source, string language, string profile)
        {
            var page = new PageView<CorrectionView>(source.Page.TotalCount, source.Page.PageSize, source.Page.PageNumber)
            {
                Data = source.Page.Data?.Select(x => Render(x))
            };

            var links = new List<LinkView>
            {
                _linkRenderer.Render(new Link {
                    ActionName = nameof(ToolController.GetCorrections),
                    Method = HttpMethod.Get,
                    Rel = RelTypes.Self,
                    Parameters = new { language = language, profile = profile },
                    QueryString = new Dictionary<string, string>()
                    {
                        { "pageNumber" , page.CurrentPageIndex.ToString() },
                        { "pageSize", page.PageSize.ToString() },
                        { "query", source.RouteArguments.Query }
                    }
                })
            };

            if (_userHelper.IsAdmin)
            {
                links.Add(_linkRenderer.Render(new Link
                {
                    ActionName = nameof(ToolController.AddCorrection),
                    Method = HttpMethod.Post,
                    Rel = RelTypes.Create,
                    Parameters = new { language = language, profile = profile}
                }));
            }

            if (page.CurrentPageIndex < page.PageCount)
            {
                links.Add(_linkRenderer.Render(new Link
                {
                    ActionName = nameof(ToolController.GetCorrections),
                    Method = HttpMethod.Get,
                    Rel = RelTypes.Next,
                    Parameters = new { language = language, profile = profile },
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
                    ActionName = nameof(ToolController.GetCorrections),
                    Method = HttpMethod.Get,
                    Rel = RelTypes.Previous,
                    Parameters = new { language = language, profile = profile},
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

        public CorrectionView Render(CorrectionModel correction)
        {
            var view = correction.Map();

            view.Links.Add(_linkRenderer.Render(new Link
            {
                ActionName = nameof(ToolController.GetCorrectionById),
                Method = HttpMethod.Get,
                Rel = RelTypes.Self,
                Parameters = new { language = correction.Language, profile = correction.Profile, incorrectText = correction.IncorrectText }
            }));


            if (_userHelper.IsAdmin)
            {
                view.Links.Add(_linkRenderer.Render(new Link
                {
                    ActionName = nameof(ToolController.UpdateCorrection),
                    Method = HttpMethod.Put,
                    Rel = RelTypes.Update,
                    Parameters = new { language = correction.Language, profile = correction.Profile, incorrectText = correction.IncorrectText }
                }));

                view.Links.Add(_linkRenderer.Render(new Link
                {
                    ActionName = nameof(ToolController.DeleteCorrection),
                    Method = HttpMethod.Delete,
                    Rel = RelTypes.Delete,
                    Parameters = new { language = correction.Language, profile = correction.Profile, incorrectText = correction.IncorrectText }
                }));
            }

            return view;
        }
    }
}
