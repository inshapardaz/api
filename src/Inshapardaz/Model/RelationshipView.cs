// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RelationshipModel.cs" company="Inshapardaz">
//   Muhammad Umer Farooq
// </copyright>
// <summary>
//   Defines the RelationshipModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;

namespace Inshapardaz.Api.Model
{
    public class RelationshipView
    {
        public int Id { get; set; }

        public int RelatedWordId { get; set; }

        public string RelatedWord { get; set; }

        public string RelationType { get; set; }

        public int RelationTypeId { get; set; }

        public IEnumerable<LinkView> Links { get; set; }
    }
}