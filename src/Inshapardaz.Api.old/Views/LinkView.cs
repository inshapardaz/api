using Newtonsoft.Json;

namespace Inshapardaz.Api.Views
{
    public class LinkView
    {
        public string Href { get; set; }

        public string Rel { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Type { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Method { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Accept { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string AcceptLanguage { get; set; }
    }
}
