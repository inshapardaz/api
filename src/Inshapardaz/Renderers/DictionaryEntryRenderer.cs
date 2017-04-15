using System.Collections.Generic;
using Inshapardaz.Helpers;
using Inshapardaz.Model;

namespace Inshapardaz.Renderers
{
    public class DictionaryEntryRenderer : RendrerBase, IRenderResponse<DictionaryEntryView>
    {
        private readonly IUserHelper _userHelper;

        public DictionaryEntryRenderer(IRenderLink linkRenderer, IUserHelper userHelper)
            : base(linkRenderer)
        {
            _userHelper = userHelper;
        }

        public DictionaryEntryView Render()
        {
            var links = new List<LinkView>
                            {
                                LinkRenderer.Render("DictionaryEntry", "self"),
                                LinkRenderer.Render("DictionaryIndex", "index", null),
                                LinkRenderer.Render("GetWordsListStartWith", "startwith", new { title = "test" }),
                                LinkRenderer.Render("WordSearch", "search", new { title = "searchTerm" }),
                                LinkRenderer.Render("GetWords", "words", null),
                                LinkRenderer.Render("GetWord", "word", new { title = "word" })
                            };

            if (_userHelper.IsAuthenticated)
            {
                links.Add(LinkRenderer.Render("CreateWord", "createWord"));
            }

            return new DictionaryEntryView { Links = links };
        }
    }
}