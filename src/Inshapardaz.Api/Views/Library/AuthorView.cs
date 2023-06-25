﻿using System.ComponentModel.DataAnnotations;

namespace Inshapardaz.Api.Views.Library
{
    public class AuthorView : ViewWithLinks
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        public int BookCount { get; set; }

        [Required]
        public string AuthorType { get; set; }
    }
}
