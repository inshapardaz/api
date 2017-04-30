// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WordDetailModel.cs" company="Inshapardaz">
//   Muhammad Umer Farooq
// </copyright>
// <summary>
//   Defines the WordDetailModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;

namespace Inshapardaz.Api.Model
{
    public class WordDetailView
    {
        public int Id { get; set; }

        public string Attributes { get; set; }

        public int AttributeValue { get; set; }

        public string Language { get; set; }

        public int LanguageId { get; set; }

        public int WordId { get; set; }

        public IEnumerable<LinkView> Links { get; set; }
       
        public IEnumerable<MeaningContextView> Meanings { get; set; }

        public IEnumerable<TranslationView> Translations { get; set; }
    }
}