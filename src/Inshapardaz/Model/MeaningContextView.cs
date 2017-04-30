using System.Collections.Generic;

namespace Inshapardaz.Api.Model
{
    public class MeaningContextView
    {
        public string Context { get; set; }
        
        public IEnumerable<LinkView> Links { get; set; }

        public IEnumerable<MeaningView> Meanings { get; set; }
    }
}
