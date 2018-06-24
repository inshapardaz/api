using System;
using System.Collections.Generic;
using System.Linq;
using Inshapardaz.Api.View;

namespace Inshapardaz.Api.Helpers
{
    public static class Extensions
    {
        public static Uri ToUri(this string url)
        {
            return new Uri(url);
        }

        public static Uri Self(this List<LinkView> source)
        {
            return source.SingleOrDefault(l => l.Rel == RelTypes.Self )?.Href;
        }
    }
}
