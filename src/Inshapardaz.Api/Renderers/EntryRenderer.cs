using System.Collections.Generic;
using Inshapardaz.Api.View;

namespace Inshapardaz.Api.Renderers
{
    public interface IRenderEntry
    {
        EntryView Render();
    }

    public class EntryRenderer : IRenderEntry
    {
        private readonly IRenderLink _linkRenderer;

        public EntryRenderer(IRenderLink linkRenderer)
        {
            _linkRenderer = linkRenderer;
        }

        public EntryView Render()
        {
            var links = new List<LinkView>
                            {
                                _linkRenderer.Render("Entry", RelTypes.Self),
                                _linkRenderer.Render("GetDictionaries", RelTypes.Dictionaries),
                                _linkRenderer.Render("GetLanguages", RelTypes.Languages),
                                _linkRenderer.Render("GetAttributes", RelTypes.Attributes),
                                _linkRenderer.Render("GetRelationTypes", RelTypes.RelationshipTypes),
                                _linkRenderer.Render("GetWordAlternatives", RelTypes.Thesaurus, new { word = "word" })
                            };
            return new EntryView { Links = links };
        }
    }
}