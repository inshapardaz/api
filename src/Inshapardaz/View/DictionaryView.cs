﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Inshapardaz.Domain.Model;

namespace Inshapardaz.Api.View
{
    public class DictionaryView
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public Languages Language { get; set; }

        public string UserId { get; set; }

        public bool IsPublic { get; set; }

        public long WordCount { get; set; }

        public IEnumerable<LinkView> Links { get; set; }

        public IEnumerable<LinkView> Indexes { get; set; }
    }
}