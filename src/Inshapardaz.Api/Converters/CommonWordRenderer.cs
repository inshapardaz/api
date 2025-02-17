using Inshapardaz.Api.Controllers;
using Inshapardaz.Api.Mappings;
using Inshapardaz.Api.Views;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Api.Views.Tools;
using Inshapardaz.Domain.Adapters;
using Inshapardaz.Domain.Adapters.Repositories;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;

namespace Inshapardaz.Api.Converters;

public interface IRenderCommonWord
{
    PageView<CommonWordView> Render(PageRendererArgs<CommonWordModel> source, string language);

    CommonWordView Render(CommonWordModel word, string language);
}

public class CommonWordRenderer : IRenderCommonWord
{
    private readonly IRenderLink _linkRenderer;
    private readonly IUserHelper _userHelper;
    private readonly IFileStorage _fileStorage;

    public CommonWordRenderer(IRenderLink linkRenderer, IUserHelper userHelper, IFileStorage fileStorage)
    {
        _linkRenderer = linkRenderer;
        _userHelper = userHelper;
        _fileStorage = fileStorage;
    }

    public PageView<CommonWordView> Render(PageRendererArgs<CommonWordModel> source, string language)
    {
        var page = new PageView<CommonWordView>(source.Page.TotalCount, source.Page.PageSize, source.Page.PageNumber)
        {
            Data = source.Page.Data?.Select(x => Render(x, language))
        };

        var links = new List<LinkView>
        {
            _linkRenderer.Render(new Link {
                ActionName = nameof(CommonWordController.GetCommonWords),
                Method = HttpMethod.Get,
                Rel = RelTypes.Self,
                Parameters = new { language = language },
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
                ActionName = nameof(CommonWordController.AddCommonWord),
                Method = HttpMethod.Post,
                Rel = RelTypes.Create,
                Parameters = new { language = language }
            }));
        }

        if (page.CurrentPageIndex < page.PageCount)
        {
            links.Add(_linkRenderer.Render(new Link
            {
                ActionName = nameof(CommonWordController.GetCommonWords),
                Method = HttpMethod.Get,
                Rel = RelTypes.Next,
                Parameters = new { language = language },
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
                ActionName = nameof(CommonWordController.GetCommonWords),
                Method = HttpMethod.Get,
                Rel = RelTypes.Previous,
                Parameters = new { language = language },
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

    public CommonWordView Render(CommonWordModel word, string language)
    {
        var view = word.Map();

        view.Links.Add(_linkRenderer.Render(new Link
        {
            ActionName = nameof(CommonWordController.GetCommonWordById),
            Method = HttpMethod.Get,
            Rel = RelTypes.Self,
            Parameters = new { language = language, id = word.Id }
        }));

        if (_userHelper.IsAdmin)
        {
            view.Links.Add(_linkRenderer.Render(new Link
            {
                ActionName = nameof(CommonWordController.UpdateCommonWord),
                Method = HttpMethod.Put,
                Rel = RelTypes.Update,
                Parameters = new { language = language, id = word.Id }
            }));

            view.Links.Add(_linkRenderer.Render(new Link
            {
                ActionName = nameof(CommonWordController.DeleteCommonWord),
                Method = HttpMethod.Delete,
                Rel = RelTypes.Delete,
                Parameters = new { language = language, id = word.Id }
            }));
        }

        return view;
    }
}
