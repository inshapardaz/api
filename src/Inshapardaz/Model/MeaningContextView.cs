using System.Collections.Generic;

namespace Inshapardaz.Model
{
    public class MeaningContextView
    {
        public string Context { get; set; }
        
        public IEnumerable<LinkView> Links { get; set; }

        public IEnumerable<MeaningView> Meanings { get; set; }
    }
}
