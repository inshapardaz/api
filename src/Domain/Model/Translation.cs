namespace Inshapardaz.Domain.Model
{
    public class Translation
    {
        public long Id { get; set; }
        public Languages Language { get; set; }
        public string Value { get; set; }
        public long WordDetailId { get; set; }

        public virtual WordDetail WordDetail { get; set; }
    }
}