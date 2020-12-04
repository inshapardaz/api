using Inshapardaz.Api.Views;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Inshapardaz.Api.Extensions
{
    public static class Extentions
    {
        public static Uri ToUri(this string url)
        {
            return new Uri(url);
        }

        public static string Self(this IEnumerable<LinkView> links)
        {
            return links.SingleOrDefault(l => l.Rel == RelTypes.Self)?.Href;
        }
    }
}
