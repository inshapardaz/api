using System;
using Newtonsoft.Json;

namespace Inshapardaz.Api.View
{
    public class LinkView
    {
        public Uri Href { get; set; }

        public string Rel { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Type { get; set; }
    }
}