using System;
using System.Linq;
using System.Threading.Tasks;

namespace Inshapardaz.Api.View.Library
{
    public class CategoryView : LinkBasedView
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public object BookCount { get; internal set; }
    }
}
