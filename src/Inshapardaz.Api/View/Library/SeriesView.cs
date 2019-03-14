using System;
using System.Linq;
using System.Threading.Tasks;

namespace Inshapardaz.Api.View.Library
{
    public class SeriesView : LinkBasedView
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }
    }
}
