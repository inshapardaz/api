using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Inshapardaz.Ports.Database.Entities.Library
{
    public class Series
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        public ICollection<SeriesCategory> SeriesCategory { get; set; } = new List<SeriesCategory>();

        public ICollection<Book> Books { get; set; } = new List<Book>();

        public int? ImageId { get; set; }
    }
}
