using System.Collections.Generic;
using Inshapardaz.Api.View;
using Inshapardaz.Domain.Helpers;

namespace Inshapardaz.Api.Renderers
{
    public interface IRenderEntry
    {
        EntryView Render();
    }

    public class EntryRenderer : IRenderEntry
    {
        private readonly IRenderLink _linkRenderer;
        private readonly IUserHelper _userHelper;

        public EntryRenderer(IRenderLink linkRenderer, IUserHelper userHelper)
        {
            _linkRenderer = linkRenderer;
            _userHelper = userHelper;
        }

        public EntryView Render()
        {
            var links = new List<LinkView>
                            {
                                _linkRenderer.Render("Entry", RelTypes.Self),
                                _linkRenderer.Render("GetDictionaries", RelTypes.Dictionaries),
                                _linkRenderer.Render("GetAuthors", RelTypes.Authors),
                                _linkRenderer.Render("GetCategories", RelTypes.Categories),
                                _linkRenderer.Render("GetSeries", RelTypes.Series),
                                _linkRenderer.Render("GetBooks", RelTypes.Books),
                                _linkRenderer.Render("GetPeriodicals", RelTypes.Periodicals),
                                _linkRenderer.Render("GetLanguages", RelTypes.Languages),
                                _linkRenderer.Render("GetAttributes", RelTypes.Attributes),
                                _linkRenderer.Render("GetRelationTypes", RelTypes.RelationshipTypes),
                                _linkRenderer.Render("GetWordAlternatives", RelTypes.Thesaurus, new { word = "word" }),
                                _linkRenderer.Render("GetLatestBooks", RelTypes.Latest)
        };
            if (_userHelper.IsAuthenticated)
            {
                links.Add(_linkRenderer.Render("GetRecentBooks", RelTypes.Recents));
                links.Add(_linkRenderer.Render("GetFavoriteBooks", RelTypes.Favorites));
                links.Add(_linkRenderer.Render("CreateFie", RelTypes.ImageUpload));
            }

            return new EntryView { Links = links };
        }
    }
}