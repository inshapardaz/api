using Newtonsoft.Json;
using System;

namespace Inshapardaz.Functions.Views
{
    public class LinkView
    {
        public string Href { get; set; }

        public string Rel { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Type { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Method { get; set; }
    }
}
