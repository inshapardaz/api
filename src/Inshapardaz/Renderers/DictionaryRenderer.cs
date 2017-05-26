using System.Collections.Generic;
using System.Linq;
using Inshapardaz.Api.Helpers;
using Inshapardaz.Api.Model;
using Inshapardaz.Domain.Model;
using Microsoft.AspNetCore.Razor.Parser.SyntaxTree;

namespace Inshapardaz.Api.Renderers
{
    public class DictionaryRenderer : RendrerBase, IRenderResponseFromObject<Dictionary, DictionaryView>
    {
        private readonly string[] _indexes = {"آ", "ا", "ب", "پ", "ت", "ٹ", "ث", "ج", "چ", "ح", "خ", "د", "ڈ", "ذ", "ر", "ڑ", "ز", "ژ", "س", "ش", "ص", "ض", "ط", "ظ", "ع", "غ", "ف"
            , "ق", "ک", "گ", "ل", "م", "ن", "و", "ہ", "ء", "ی" };

        private readonly IUserHelper _userHelper;

        public DictionaryRenderer(IRenderLink linkRenderer, IUserHelper userHelper)
            : base(linkRenderer)
        {
            _userHelper = userHelper;
        }

        public DictionaryView Render(Dictionary source)
        {
            var links = new List<LinkView>
                            {
                                LinkRenderer.Render("GetDictionaryById", "self", new { id = source.Id }),
                                LinkRenderer.Render("GetWords", "index", new { id = source.Id }),
                                LinkRenderer.Render("SearchDictionary", "search", new { id = source.Id })
            };

            if (_userHelper.IsContributor)
            {
                links.Add(LinkRenderer.Render("UpdateDictionary", "update", new { id = source.Id }));
                links.Add(LinkRenderer.Render("DeleteDictionary", "delete", new { id = source.Id }));
                links.Add(LinkRenderer.Render("CreateWord", "create-word", new { id = source.Id }));
            }

            var indexes = new List<LinkView>(_indexes.Select(i => LinkRenderer.Render("GetWordsListStartWith", i, new { id = source.Id, startingWith = i })));

            var result = source.Map<Dictionary, DictionaryView>();
            result.Links = links;
            result.Indexes = indexes;
            return result;
        }
    }
}