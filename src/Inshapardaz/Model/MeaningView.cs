// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MeaningModel.cs" company="Inshapardaz">
//   Muhammad Umer Farooq
// </copyright>
// <summary>
//   Defines the MeaningModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Inshapardaz.Api.Model
{
    public class MeaningView
    {
        public long Id { get; set; }

        public string Context { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string Value { get; set; }

        public string Example { get; set; }

        public long WordDetailId { get; set; }

        public IEnumerable<LinkView> Links { get; set; }
    }
}