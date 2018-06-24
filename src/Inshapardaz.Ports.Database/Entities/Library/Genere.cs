using System.ComponentModel.DataAnnotations;

namespace Inshapardaz.Ports.Database.Entities.Library
{
    public class Genere
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
    }
}