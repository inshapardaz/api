﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Inshapardaz.Api.View
{
    public class SpellCheckResultView
    {
        public string Word { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public LinkView WordLink { get; set; }

        public int IndexInString { get; set; }

        public bool Correct { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public IEnumerable<SpellingOption> Corrections { get; set; }
    }

    public class SpellingOption
    {
        public string Title { get; set; }

        public IEnumerable<LinkView> Links { get; set; }
    }
}
