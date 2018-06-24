using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inshapardaz.Api.View.Library
{
    public class GenereView
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public IEnumerable<LinkView> Links { get; set; }
    }
}
