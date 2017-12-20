using Inshapardaz.Domain.Database.Entities;

namespace Inshapardaz.Api.View
{
    public class TranspileRequestView
    {
        public Languages FromLanguage { get; set; }

        public Languages ToLanguage { get; set; }

        public string Source { get; set; }
    }
}