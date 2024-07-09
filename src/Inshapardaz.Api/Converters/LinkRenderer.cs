using Inshapardaz.Api.Views;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.AspNetCore.Hosting;

namespace Inshapardaz.Api.Converters;

public class Link
{
    public string ActionName { get; set; }
    public string Rel { get; set; }
    public string Type { get; set; }
    public HttpMethod Method { get; set; }
    public string MimeType { get; set; }
    public string Language { get; set; }
    public object Parameters { get; set; }

    public Dictionary<string, string> QueryString { get; set; } = new Dictionary<string, string>();
}

public interface IRenderLink
{
    LinkView Render(Link link);

    //LinkView Render(string methodName, string rel);

    //LinkView Render(string methodName, string rel, object data);

    //LinkView Render(string methodName, string rel, string type, object data);
}

public class LinkRenderer : IRenderLink
{
    private readonly string _environment;
    private readonly IActionContextAccessor _actionContextAccessor;
    private readonly ILogger<LinkRenderer> _log;
    private readonly IUrlHelper _urlHelper;

    public LinkRenderer(IUrlHelperFactory urlHelperFactory, 
        IWebHostEnvironment hostingEnvironment,
        IActionContextAccessor actionContextAccessor, 
        ILogger<LinkRenderer> log)
    {
        _environment = hostingEnvironment.EnvironmentName;
        _actionContextAccessor = actionContextAccessor;
        _urlHelper = urlHelperFactory.GetUrlHelper(actionContextAccessor.ActionContext);
        _log = log;
    }

    public LinkView Render(Link link)
    {
        var urlBuilder = new UriBuilder(_urlHelper.RouteUrl(link.ActionName, link.Parameters, Scheme));
        var param = new Dictionary<string, string>();

        if (link.QueryString != null && link.QueryString.Any())
        {
            foreach (var item in link.QueryString)
            {
                if (!string.IsNullOrWhiteSpace(item.Value))
                {
                    param[item.Key] = item.Value;
                }
            }
        }

        return new LinkView
        {
            Href = QueryHelpers.AddQueryString(urlBuilder.Uri.ToString(), param),
            Rel = link.Rel,
            Type = link.Type,
            Method = link.Method.ToString(),
            AcceptLanguage = link.Language,
            Accept = link.MimeType
        };
    }

    private string Scheme => _environment.Equals("production", StringComparison.InvariantCultureIgnoreCase) ? "https" : "http";
}
