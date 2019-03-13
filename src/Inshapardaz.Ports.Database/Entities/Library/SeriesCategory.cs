namespace Inshapardaz.Ports.Database.Entities.Library
{
    public class SeriesCategory
    {
        public int SeriesId { get; set; }

        public Series Series { get; set; }

        public int CategoryId { get; set; }

        public Category Category { get; set; }
    }
}