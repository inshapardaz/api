using Inshapardaz.Functions.Views;
using System.Collections.Generic;
using System.Linq;

namespace Inshapardaz.Functions.Extensions
{
    public static class Extentions
    {
        public static string Self(this IEnumerable<LinkView> links)
        {
            return links.SingleOrDefault(l => l.Rel == RelTypes.Self)?.Href;
        }
    }
}
