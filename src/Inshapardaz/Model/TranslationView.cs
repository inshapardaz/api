// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TranslationModel.cs" company="Inshapardaz">
//   Muhammad Umer Farooq
// </copyright>
// <summary>
//   Defines the TranslationModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;

namespace Inshapardaz.Api.Model
{
    public class TranslationView
    {
        public int Id { get; set; }

        public string Language { get; set; }

        public int LanguageId { get; set; }

        public IEnumerable<LinkView> Links { get; internal set; }

        public string Value { get; set; }

        public int WordId { get; set; }
    }
}