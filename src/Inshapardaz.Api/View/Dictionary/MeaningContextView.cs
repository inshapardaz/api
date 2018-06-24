using System.Collections.Generic;

namespace Inshapardaz.Api.View.Dictionary
{
    public class MeaningContextView
    {
        public string Context { get; set; }
        
        public IEnumerable<LinkView> Links { get; set; }

        public IEnumerable<MeaningView> Meanings { get; set; }
    }
}
