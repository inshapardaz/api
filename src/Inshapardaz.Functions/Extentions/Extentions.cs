using System.Collections.Generic;
using System.Linq;
using Inshapardaz.Functions.Views;

namespace Inshapardaz.Functions.Extentions
{
    public static class Extentions
    {
        public static string Self(this IEnumerable<LinkView> links) 
        {
            return links.SingleOrDefault(l => l.Rel == RelTypes.Self)?.Href;
        }       
    }
}