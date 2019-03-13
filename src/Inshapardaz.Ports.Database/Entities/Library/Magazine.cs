using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Inshapardaz.Ports.Database.Entities.Library
{
    public class Magazine
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        public string Description { get; set; }

        public int? ImageId { get; set; }

        public int? Category { get; set; }

        public virtual MagazineCategory MagazineCategory { get; set; }
    }

    public class MagazineCategory
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public ICollection<Magazine> Magazines { get; set; } = new List<Magazine>();
    }
}
