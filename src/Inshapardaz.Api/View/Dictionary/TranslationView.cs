// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TranslationModel.cs" company="Inshapardaz">
//   Muhammad Umer Farooq
// </copyright>
// <summary>
//   Defines the TranslationModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Inshapardaz.Api.View.Dictionary
{
    public class TranslationView
    {
        public long Id { get; set; }

        public string Language { get; set; }

        [Required(AllowEmptyStrings = false)]
        public int LanguageId { get; set; }

        public IEnumerable<LinkView> Links { get; set; }

        [Required]
        public string Value { get; set; }

        public long WordId { get; set; }

        public bool IsTranspiling { get; set; }
    }
}