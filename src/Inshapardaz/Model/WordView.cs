// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WordModel.cs" company="Inshapardaz">
//     Muhammad Umer Farooq
// </copyright>
// <summary>
// Defines the WordModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Inshapardaz.Api.Model
{
    public class WordView
    {
        public string Description { get; set; }

        public long Id { get; set; }

        public IEnumerable<LinkView> Links { get; set; }

        [Required(AllowEmptyStrings = false)]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string Title { get; set; }

        public string TitleWithMovements { get; set; }

        public string Pronunciation { get; set; }
    }
}