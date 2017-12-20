using Inshapardaz.Domain.Database.Entities;

namespace Inshapardaz.Api.View
{
    public class TranspileResponseView
    {
        public Languages FromLanguage { get; set; }

        public Languages ToLanguage { get; set; }

        public string Result { get; set; }
    }
}