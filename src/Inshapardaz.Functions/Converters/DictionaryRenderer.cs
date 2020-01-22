using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Inshapardaz.Functions.Authentication;
using Inshapardaz.Functions.Dictionaries;
using Inshapardaz.Functions.Dictionaries.Words;
using Inshapardaz.Functions.Mappings;
using Inshapardaz.Functions.Views;
using Inshapardaz.Functions.Views.Dictionaries;

namespace Inshapardaz.Functions.Converters
{
    public static class DictionaryRenderer
    {
        public static ListView<DictionaryView> Render(this IEnumerable<Domain.Models.Dictionaries.DictionaryModel> source, ClaimsPrincipal principal)
        {
            var items = source.Select(g => g.Render(principal));
            var view = new ListView<DictionaryView> { Items = items };
            view.Links.Add(GetDictionaries.Link(RelTypes.Self));

            if (principal.IsWriter())
            {
                view.Links.Add(AddDictionary.Link(RelTypes.Create));
            }

            return view;
        }

        public static DictionaryView Render(this Domain.Models.Dictionaries.DictionaryModel source, ClaimsPrincipal principal)
        {
            var result = source.Map();

            var links = new List<LinkView>
            {
                 GetDictionaryById.Link(source.Id, RelTypes.Self),
                 GetWordsByDictionary.Link(source.Id, RelTypes.Index),
                 GetWordsByDictionary.Link(source.Id, RelTypes.Search)
            };

            if (principal.IsWriter())
            {
                links.Add(UpdateDictionary.Link(source.Id, RelTypes.Update));
                links.Add(DeleteDictionary.Link(source.Id, RelTypes.Delete));
                links.Add(CreateDictionaryDownload.Link(source.Id, RelTypes.CreateDownload));
                links.Add(AddWord.Link(source.Id, RelTypes.CreateWord));
            }

            if (source.Downloads != null && (source.IsPublic || principal.IsWriter()))
            {
                foreach (var download in source.Downloads)
                {
                    links.Add(GetDictionaryDownloadById.Link(source.Id, download.MimeType, RelTypes.Download));
                }
            }

            result.Links = links;
            return result;
        }

        private static readonly string[] Indexes =
        {
            "آ", "ا", "ب", "پ", "ت", "ٹ", "ث", "ج", "چ", "ح", "خ", "د", "ڈ", "ذ", "ر", "ڑ", "ز", "ژ", "س", "ش", "ص",
            "ض", "ط", "ظ", "ع", "غ", "ف", "ق", "ک", "گ", "ل", "م", "ن", "و", "ہ", "ء", "ی"
        };
    }
}
