using Nest;

namespace Inshapardaz.Domain.Entities
{
    public class Translation
    {
        public long Id { get; set; }
        public Languages Language { get; set; }
        [Text(Ignore = true)]
        public string Value { get; set; }
        public long WordId { get; set; }
        public bool IsTrasnpiling { get; set; }
    }
}