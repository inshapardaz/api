using Inshapardaz.Api.Helpers;
using Inshapardaz.Api.View;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Logging;

namespace Inshapardaz.Api.Renderers
{
    public class LinkRenderer : IRenderLink
    {
        private readonly IActionContextAccessor _actionContextAccessor;
        private readonly ILogger<LinkRenderer> _log;
        private readonly IUrlHelper _urlHelper;

        public LinkRenderer(IUrlHelperFactory urlHelperFactory, IActionContextAccessor actionContextAccessor, ILogger<LinkRenderer> log)
        {
            _actionContextAccessor = actionContextAccessor;
            _urlHelper = urlHelperFactory.GetUrlHelper(actionContextAccessor.ActionContext);
            _log = log;
        }

        private string Scheme => _actionContextAccessor.ActionContext.HttpContext.Request.Scheme;

        public LinkView Render(string methodName, string rel)
        {
            _log.LogTrace("Rendering link for {0}, with rel {1}", methodName, rel);
            return new LinkView { Href = _urlHelper.RouteUrl(methodName, null, Scheme).ToUri(), Rel = rel };
        }

        public LinkView Render(string methodName, string rel, object data)
        {
            _log.LogTrace("Rendering link for {0}, with rel {1} and data", methodName, rel, data);
            return new LinkView { Href = _urlHelper.RouteUrl(methodName, data, Scheme).ToUri(), Rel = rel };
        }
    }
}