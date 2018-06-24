namespace Inshapardaz.Domain.Entities.Dictionary
{
    public class Meaning
    {
        public long Id { get; set; }

        public string Context { get; set; }

        public string Value { get; set; }

        public string Example { get; set; }

        public long WordId { get; set; }

        public virtual Word Word { get; set; }
    }
}