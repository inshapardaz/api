// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RelationshipModel.cs" company="Inshapardaz">
//   Muhammad Umer Farooq
// </copyright>
// <summary>
//   Defines the RelationshipModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Inshapardaz.Api.View.Dictionary
{
    public class RelationshipView
    {
        public long Id { get; set; }

        [Required]
        public long SourceWordId { get; set; }

        public string SourceWord { get; set; }

        [Required]
        public long RelatedWordId { get; set; }

        public string RelatedWord { get; set; }

        public string RelationType { get; set; }

        [Required]
        public int RelationTypeId { get; set; }

        public IEnumerable<LinkView> Links { get; set; }
    }
}