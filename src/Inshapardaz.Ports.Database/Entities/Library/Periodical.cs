using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Inshapardaz.Ports.Database.Entities.Library
{
    public class Periodical
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        public string Description { get; set; }

        public int? ImageId { get; set; }

        public int? CategoryId { get; set; }

        public virtual PeriodicalCategory Category { get; set; }

        public virtual ICollection<Issue> Issues { get; set; }
    }

    public class PeriodicalCategory
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public ICollection<Periodical> Periodicals { get; set; } = new List<Periodical>();
    }
}
