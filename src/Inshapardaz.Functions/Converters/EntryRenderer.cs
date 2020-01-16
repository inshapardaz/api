using System.Collections.Generic;
using System.Security.Claims;
using Inshapardaz.Functions.Authentication;
using Inshapardaz.Functions.Dictionaries;
using Inshapardaz.Functions.Library.Authors;
using Inshapardaz.Functions.Library.Books;
using Inshapardaz.Functions.Library.Categories;
using Inshapardaz.Functions.Library.Files;
using Inshapardaz.Functions.Library.Periodicals;
using Inshapardaz.Functions.Library.Series;
using Inshapardaz.Functions.Views;

namespace Inshapardaz.Functions.Converters
{ 
    public static class EntryRenderer
    {        
        public static EntryView Render(ClaimsPrincipal user)
        {
            var links = new List<LinkView>
                            {
                                GetEntry.Link(),
                                GetAuthors.Link(RelTypes.Authors),
                                GetCategories.Link(RelTypes.Categories),
                                GetSeries.Link(RelTypes.Series),
                                GetBooks.Link(RelTypes.Books),
                                GetPeriodicals.Link(RelTypes.Periodicals),
                                GetLanguages.Link(RelTypes.Languages),
                                GetAttributes.Link(RelTypes.Attributes),
                                GetRelationTypes.Link(RelTypes.RelationshipTypes),
                                GetDictionaries.Link(RelTypes.Dictionaries),
                                GetLatestBooks.Link(RelTypes.Latest)
        };
            if (user.IsAuthenticated())
            {
                 links.Add(GetRecentReadBooks.Link(RelTypes.Recents));
                 links.Add(GetFavoriteBooks.Link(RelTypes.Favorites));
            }

            if (user.IsWriter())
            {
                links.Add(AddFile.Link(RelTypes.ImageUpload));
            }

            return new EntryView { Links = links };
        }
    }
}
