namespace Inshapardaz.Domain.Database.Entities
{
    public class Translation
    {
        public long Id { get; set; }
        public Languages Language { get; set; }
        public string Value { get; set; }
        public long WordId { get; set; }
        public bool IsTrasnpiling { get; set; }

        public virtual Word Word { get; set; }
    }
}