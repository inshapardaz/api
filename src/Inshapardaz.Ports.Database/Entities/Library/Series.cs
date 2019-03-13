using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Inshapardaz.Ports.Database.Entities.Library
{
    public class Series
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        public ICollection<SeriesCategory> SeriesCategory { get; set; } = new List<SeriesCategory>();

        public int? ImageId { get; set; }
    }
}
