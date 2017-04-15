using System.Collections.Generic;
using Inshapardaz.Model;

namespace Inshapardaz.Renderers
{
    public class EntryRenderer : RendrerBase, IRenderResponse<EntryView>
    {
        public EntryRenderer(IRenderLink linkRenderer)
            : base(linkRenderer)
        {
        }

        public EntryView Render()
        {
            var links = new List<LinkView>
                            {
                                LinkRenderer.Render("Entry", RelTypes.Self),
                                LinkRenderer.Render("GetDictionaries", RelTypes.Dictionaries),
                                LinkRenderer.Render("GetLanguages", RelTypes.Languages),
                                LinkRenderer.Render("GetAttributes", RelTypes.Attributes),
                                LinkRenderer.Render("GetRelationTypes", RelTypes.RelationshipTypes),
                                LinkRenderer.Render("GetWordAlternatives", RelTypes.Thesaurus, new { word = "word" })
                            };
            return new EntryView { Links = links };
        }
    }
}