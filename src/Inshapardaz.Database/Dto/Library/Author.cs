using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Inshapardaz.Database.Dto.Library
{
    public class AuthorDto
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int? ImageId { get; set; }
    }
}
