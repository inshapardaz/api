using Inshapardaz.Api.Views;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Inshapardaz.Api.Converters
{
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
        private readonly IActionContextAccessor _actionContextAccessor;
        private readonly ILogger<LinkRenderer> _log;
        private readonly IUrlHelper _urlHelper;

        public LinkRenderer(IUrlHelperFactory urlHelperFactory, IActionContextAccessor actionContextAccessor, ILogger<LinkRenderer> log)
        {
            _actionContextAccessor = actionContextAccessor;
            _urlHelper = urlHelperFactory.GetUrlHelper(actionContextAccessor.ActionContext);
            _log = log;
        }

        public LinkView Render(Link link)
        {
            var urlBuilder = new UriBuilder()
            {
                Path = _urlHelper.RouteUrl(link.ActionName, link.Parameters, Scheme)
            };

            if (link.QueryString != null && link.QueryString.Any())
            {
                var collection = HttpUtility.ParseQueryString(string.Empty);
                foreach (var item in link.QueryString)
                {
                    collection[item.Key] = item.Value;
                }

                urlBuilder.Query = collection.ToString();
            }

            return new LinkView
            {
                Href = urlBuilder.Uri.ToString(),
                Rel = link.Rel,
                Type = link.Type,
                Method = link.Method.ToString(),
                AcceptLanguage = link.Language,
                Accept = link.MimeType
            };
        }

        private string Scheme => _actionContextAccessor.ActionContext.HttpContext.Request.Scheme;

        //public LinkView Render(string methodName, string rel)
        //{
        //    _log.LogTrace($"Rendering link for {methodName}, with rel {rel}");
        //    return new LinkView { Href = _urlHelper.RouteUrl(methodName, null, Scheme), Rel = rel };
        //}

        //public LinkView Render(string methodName, string rel, object data)
        //{
        //    _log.LogTrace($"Rendering link for {methodName}, with rel {rel} and {data}");
        //    return new LinkView { Href = _urlHelper.RouteUrl(methodName, data, Scheme), Rel = rel };
        //}

        //public LinkView Render(string methodName, string rel, string type, object data)
        //{
        //    _log.LogTrace($"Rendering link for {methodName}, with rel {rel}, type {type} and {data}");
        //    return new LinkView { Href = _urlHelper.RouteUrl(methodName, data, Scheme), Rel = rel, Type = type };
        //}
    }
}
