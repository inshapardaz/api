using System.Collections.Generic;
using Inshapardaz.Functions.Authentication;
using Inshapardaz.Functions.Dictionary;
using Inshapardaz.Functions.Library.Authors;
using Inshapardaz.Functions.Library.Books;
using Inshapardaz.Functions.Library.Categories;
using Inshapardaz.Functions.Library.Periodicals;
using Inshapardaz.Functions.Library.Series;
using Inshapardaz.Functions.Views;

namespace Inshapardaz.Functions.Adapters
{
    public interface IRenderEntry
    {
        EntryView Render();
    }

    public class EntryRenderer : IRenderEntry
    {
        private readonly IAuthenticationHelper _authenticationHelper;

        public EntryRenderer(IAuthenticationHelper authenticationHelper)
        {
            _authenticationHelper = authenticationHelper;
        }
        
        public EntryView Render()
        {
            var links = new List<LinkView>
                            {
                                GetEntry.Self(),
                                GetAuthors.Self(RelTypes.Authors),
                                GetCategories.Self(RelTypes.Categories),
                                GetSeries.Self(RelTypes.Series),
                                GetBooks.Self(RelTypes.Books),
                                GetPeriodicals.Self(RelTypes.Periodicals),
                                GetLanguages.Self(RelTypes.Languages),
                                GetAttributes.Self(RelTypes.Attributes),
                                GetRelationTypes.Self(RelTypes.RelationshipTypes),
                                //_linkRenderer.Render("GetDictionaries", RelTypes.Dictionaries),
                                //_linkRenderer.Render("GetWordAlternatives", RelTypes.Thesaurus, new { word = "word" }),
                                GetLatestBooks.Self(RelTypes.Latest)
        };
            // if (_userHelper.IsAuthenticated)
            // {
            //     links.Add(_linkRenderer.Render("GetRecentBooks", RelTypes.Recents));
            //     links.Add(_linkRenderer.Render("GetFavoriteBooks", RelTypes.Favorites));
            //     links.Add(_linkRenderer.Render("CreateFie", RelTypes.ImageUpload));
            // }

            return new EntryView { Links = links };
        }
    }
}