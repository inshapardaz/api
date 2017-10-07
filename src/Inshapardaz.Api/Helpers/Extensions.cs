using System;

namespace Inshapardaz.Api.Helpers
{
    public static class Extensions
    {
        public static Uri ToUri(this string url)
        {
            return new Uri(url);
        }
    }
}
