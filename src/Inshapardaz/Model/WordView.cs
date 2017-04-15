// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WordModel.cs" company="Inshapardaz">
//     Muhammad Umer Farooq
// </copyright>
// <summary>
// Defines the WordModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;

namespace Inshapardaz.Model
{
    public class WordView
    {
        public string Description { get; set; }

        public int Id { get; set; }

        public IEnumerable<LinkView> Links { get; set; }

        public string Title { get; set; }

        public string TitleWithMovements { get; set; }

        public string Pronunciation { get; set; }
    }
}