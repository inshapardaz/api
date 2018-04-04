using Nest;

namespace Inshapardaz.Domain.Entities
{
    public class Meaning
    {
        public long Id { get; set; }
        [Text(Ignore = true)]
        public string Context { get; set; }
        [Text(Ignore = true)]
        public string Value { get; set; }
        [Text(Ignore = true)]
        public string Example { get; set; }
        public long WordId { get; set; }
    }
}