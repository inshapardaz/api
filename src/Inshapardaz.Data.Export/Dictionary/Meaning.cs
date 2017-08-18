namespace Inshapardaz.Data.Dictionary
{
    public class Meaning
    {
        public long Id { get; set; }
        public string Context { get; set; }
        public string Value { get; set; }
        public string Example { get; set; }
        public long WordDetailId { get; set; }

        public virtual WordDetail WordDetail { get; set; }
    }
}