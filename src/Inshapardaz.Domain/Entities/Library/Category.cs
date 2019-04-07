namespace Inshapardaz.Domain.Entities.Library
{
    public class Category
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int BookCount { get; internal set; }
    }
}